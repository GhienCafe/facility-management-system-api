using System.ComponentModel.DataAnnotations;
using AppCore.Models;

namespace AppCore.Attributes;

public class MinValueAttribute : ValidationAttribute
{
    public MinValueAttribute(double minValue)
    {
        MinValue = minValue;
    }

    private double MinValue { get; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if ((double)value < MinValue)
            throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);
        return ValidationResult.Success;
    }
}