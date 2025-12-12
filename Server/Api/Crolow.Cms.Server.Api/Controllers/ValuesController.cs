using Crolow.Apps.Common.Reflection;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Api.Controllers
{
    [Route("api/kalow/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var x = new string[] { "value1", "value2" };
            var y = x.EvalueExpression<string>("o[0] + \".\" + o[1]");
            return new[] { y };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}