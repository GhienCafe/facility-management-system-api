using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppCore.Models;

namespace AppCore.Attributes;

public class MinYearAttribute : ValidationAttribute
{
    private int MinYear { get; }

    public MinYearAttribute(int minYear)
    {
        MinYear = minYear;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            var now = DateTime.Now;
            var datetime = (DateTime)value;
            if (datetime.AddYears(MinYear).Date >= now.Date)
                throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);

            return ValidationResult.Success;
        }
        catch (RegexMatchTimeoutException)
        {
            throw new ApiException(StatusCode.TIME_OUT);
        }
        catch (Exception)
        {
            throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);
        }
    }
}