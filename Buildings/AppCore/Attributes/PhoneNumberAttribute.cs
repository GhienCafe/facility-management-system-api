using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppCore.Models;

namespace AppCore.Attributes;

public class PhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            var stringValue = (string) value;
            var validatePhoneNumberRegex = new Regex(
                "(84|[03|05|07|08|09]|[3|5|7|8|9])+([0-9]{7})",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(20)
            );
            var isMatch = validatePhoneNumberRegex.IsMatch(stringValue);
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