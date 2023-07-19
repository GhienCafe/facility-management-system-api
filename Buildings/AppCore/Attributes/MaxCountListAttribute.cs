using System.ComponentModel.DataAnnotations;
using AppCore.Models;

namespace AppCore.Attributes;

public class MaxCountListAttribute : ValidationAttribute
{
    public MaxCountListAttribute(int length)
    {
        Length = length;
    }

    private int Length { get; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is List<object> list && list.Count > Length)
        {
            throw new ApiException(FormatErrorMessage(validationContext.DisplayName), StatusCode.BAD_REQUEST);
        }

        return ValidationResult.Success;
    }
}