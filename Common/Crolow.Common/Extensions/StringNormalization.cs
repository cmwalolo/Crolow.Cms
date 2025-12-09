using System;
using System.Globalization;
using System.Text;

namespace Crolow.Apps.Common.Extensions;

public static class StringNormalizer
{
    public static string NormalizeString(this string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        // Normalize the string to FormD (decomposed form), which separates characters from their diacritical marks.
        string normalizedString = input.Normalize(NormalizationForm.FormD);

        // Create a StringBuilder to hold the normalized ASCII characters.
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            // Check if the character is a non-spacing mark (diacritical mark).
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                // Append the character to the StringBuilder if it's not a diacritical mark.
                stringBuilder.Append(c);
            }
        }

        // Convert the StringBuilder to a string and normalize it to FormC (composed form).
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
