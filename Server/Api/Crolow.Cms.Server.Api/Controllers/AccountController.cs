using Asp.Versioning;
using Crolow.Cms.Server.Api.Model;
using Crolow.Cms.Server.Api.Services;
using Crolow.Cms.Server.Managers.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;



namespace Crolow.Cms.Server.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/crolow/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;


    public AccountController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _smsSender = smsSender;
        _logger = loggerFactory.CreateLogger<AccountController>();
    }


    #region Admin Methods

    [HttpPost]
    [Authorize(Policy = "admin")]
    [Route("createrole")]
    public async Task<bool> CreateRole(string name)
    {
        var role = new ApplicationRole { Name = name };
        var result = await _roleManager.CreateAsync(role);
        return result.Succeeded;
    }

    [HttpPost]
    [Authorize(/*Roles = "admin"*/   Policy = "admin")]
    [Route("getroles")]
    public List<ApplicationRole> GetRoles(string name)
    {
        var role = new ApplicationRole { Name = name };
        return _roleManager.Roles.ToList();
    }

    #endregion Admin Methods

    #region Authentication / Account API

    // POST: /api/kalow/account/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] IdentityModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return Ok(new
            {
                success = true,
                requiresTwoFactor = false,
                isLockedOut = false
            });
        }

        if (result.RequiresTwoFactor)
        {
            return Ok(new
            {
                success = false,
                requiresTwoFactor = true,
                isLockedOut = false
            });
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return Unauthorized(new
            {
                success = false,
                isLockedOut = true,
                error = "User account locked out."
            });
        }

        return Unauthorized(new
        {
            success = false,
            error = "Invalid login attempt."
        });
    }

    // POST: /api/kalow/account/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] IdentityModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // optional: send confirmation email here with a URL pointing to your frontend
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User created a new account with password.");

            return Ok(new
            {
                success = true,
                userId = user.Id.ToString(),
                email = user.Email
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors.Select(e => e.Description).ToArray()
        });
    }

    // POST: /api/kalow/account/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return Ok(new { success = true });
    }

    #endregion Authentication / Account API

    #region External Login API

    // POST: /api/kalow/account/externallogin
    // Kicks off external provider auth; typical for web clients, still returns a Challenge (302).
    [HttpPost("externallogin")]
    [AllowAnonymous]
    public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    // GET: /api/kalow/account/externallogincallback
    [HttpGet("externallogincallback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback([FromQuery] string? returnUrl = null, [FromQuery] string? remoteError = null)
    {
        if (remoteError != null)
        {
            return BadRequest(new { error = $"Error from external provider: {remoteError}" });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest(new { error = "External login info not available." });
        }

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false);

        if (result.Succeeded)
        {
            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

            _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
            return Ok(new
            {
                success = true,
                provider = info.LoginProvider,
                requiresTwoFactor = false,
                isLockedOut = false
            });
        }

        if (result.RequiresTwoFactor)
        {
            return Ok(new
            {
                success = false,
                requiresTwoFactor = true,
                isLockedOut = false
            });
        }

        if (result.IsLockedOut)
        {
            return Unauthorized(new
            {
                success = false,
                isLockedOut = true,
                error = "User account locked out."
            });
        }

        // need to create a local account
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        return Ok(new
        {
            success = false,
            requiresAccountCreation = true,
            loginProvider = info.LoginProvider,
            email
        });
    }

    // POST: /api/kalow/account/externalloginconfirmation
    [HttpPost("externalloginconfirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginConfirmation([FromBody] ExternalLoginConfirmationViewModel model, [FromQuery] string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest(new { error = "External login info not available." });
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                return Ok(new
                {
                    success = true,
                    userId = user.Id.ToString(),
                    email = user.Email
                });
            }
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors.Select(e => e.Description).ToArray()
        });
    }

    #endregion External Login API

    #region Email confirmation / password reset API

    // GET: /api/kalow/account/confirmemail
    [HttpGet("confirmemail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            return BadRequest(new { error = "UserId and code are required." });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { error = "User not found." });

        var result = await _userManager.ConfirmEmailAsync(user, code);
        return Ok(new { success = result.Succeeded });
    }

    // POST: /api/kalow/account/forgotpassword
    [HttpPost("forgotpassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // don't reveal existence
            return Ok(new { success = true });
        }

        // Generate token & send email here
        // var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        // var callbackUrl = $"{frontendUrl}/reset-password?userId={user.Id}&code={Uri.EscapeDataString(code)}";
        // await _emailSender.SendEmailAsync(model.Email, "Reset Password", "...");

        return Ok(new { success = true });
    }

    // POST: /api/kalow/account/resetpassword
    [HttpPost("resetpassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // don't reveal that the user does not exist
            return Ok(new { success = true });
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors.Select(e => e.Description).ToArray()
        });
    }

    #endregion Email confirmation / password reset API

    #region Two-factor Authentication API

    // GET: /api/kalow/account/2fa/providers
    [HttpGet("2fa/providers")]
    [Authorize]
    public async Task<IActionResult> GetTwoFactorProviders()
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
            return Unauthorized(new { error = "Two-factor user not found." });

        var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
        return Ok(new { providers });
    }

    // POST: /api/kalow/account/2fa/sendcode
    [HttpPost("2fa/sendcode")]
    [AllowAnonymous]
    public async Task<IActionResult> SendCode([FromBody] SendCodeViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
            return Unauthorized(new { error = "Two-factor user not found." });

        if (model.SelectedProvider == "Authenticator")
        {
            return Ok(new
            {
                success = true,
                redirect = "verify-authenticator" // up to frontend to interpret
            });
        }

        var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
        if (string.IsNullOrWhiteSpace(code))
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unable to generate 2FA code." });

        var message = "Your security code is: " + code;

        if (model.SelectedProvider == "Email")
        {
            await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
        }
        else if (model.SelectedProvider == "Phone")
        {
            await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
        }

        return Ok(new { success = true });
    }

    // POST: /api/kalow/account/2fa/verifycode
    [HttpPost("2fa/verifycode")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.TwoFactorSignInAsync(
            model.Provider,
            model.Code,
            model.RememberMe,
            model.RememberBrowser);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return Unauthorized(new
            {
                success = false,
                isLockedOut = true,
                error = "User account locked out."
            });
        }

        return Unauthorized(new { success = false, error = "Invalid code." });
    }

    // POST: /api/kalow/account/2fa/verifyauthenticator
    [HttpPost("2fa/verifyauthenticator")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyAuthenticatorCode([FromBody] VerifyAuthenticatorCodeViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            model.Code,
            model.RememberMe,
            model.RememberBrowser);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return Unauthorized(new
            {
                success = false,
                isLockedOut = true,
                error = "User account locked out."
            });
        }

        return Unauthorized(new { success = false, error = "Invalid code." });
    }

    // POST: /api/kalow/account/2fa/userecoverycode
    [HttpPost("2fa/userecoverycode")]
    [AllowAnonymous]
    public async Task<IActionResult> UseRecoveryCode([FromBody] UseRecoveryCodeViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(model.Code);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        return Unauthorized(new { success = false, error = "Invalid recovery code." });
    }

    #endregion Two-factor Authentication API

    #region Tokenizer

    [HttpPost]
    [Route("gettoken")]
    public IActionResult GetToken([FromBody] IdentityModel model)
    {
        if (_signInManager.PasswordSignInAsync(model.Name, model.Password, false, lockoutOnFailure: false).Result.Succeeded)
        {
            var result = _userManager.FindByNameAsync(model.Name).Result;
            if (result != null)
            {
                return new JsonResult(GenerateToken(result));
            }
        }
        return BadRequest();
    }


    private string GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.SerialNumber, user.Id.ToString())
                    };

        var s = ClaimTypes.Role;
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        var key = TokenSecurityKey.Create(_configuration["JWTToken"]);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Kalow.Apps.Bearer",
            audience: "Kalow.Apps.Bearer",
            claims: claims,
            expires: DateTime.Now.AddMinutes(1400),
            signingCredentials: creds);

        var sectoken = new JwtSecurityTokenHandler().WriteToken(token);
        return sectoken;
    }
    #endregion Tokenizer
}
