using System.ComponentModel.DataAnnotations;
using AppCore.Models;

namespace AppCore.Attributes;

public class MaxLengthAttribute : ValidationAttribute
{
    public MaxLengthAttribute(int length)
    {
        Length = length;
    }

    private int Length { get; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value.ToString()?.Length > Length)
            throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);
        return ValidationResult.Success;
    }
}