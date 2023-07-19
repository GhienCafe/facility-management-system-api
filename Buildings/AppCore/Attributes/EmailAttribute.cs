using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppCore.Models;

namespace AppCore.Attributes;

public class EmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            var stringValue = (string) value;
            var validateEmailRegex = new Regex(
                "^\\S+@\\S+\\.\\S+$",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(20)
            );
            var isMatch = validateEmailRegex.IsMatch(stringValue);
            if (stringValue == null ||
                string.IsNullOrEmpty(stringValue) ||
                stringValue.Length == 0 ||
                !isMatch
               )
            {
                throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);
            }

            return ValidationResult.Success;
        }
        catch (RegexMatchTimeoutException)
        {
            throw new ApiException(StatusCode.TIME_OUT);
        }
        catch
        {
            throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);
        }
    }
}