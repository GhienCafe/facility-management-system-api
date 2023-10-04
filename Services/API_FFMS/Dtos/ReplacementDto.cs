using System.ComponentModel.DataAnnotations;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class QuantityGreaterThanZeroAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int quantity && quantity > 0)
        {
            return ValidationResult.Success!;
        }

        return new ValidationResult("Số lượng phải lớn hơn 0");
    }
}
