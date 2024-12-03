using System.ComponentModel.DataAnnotations;

namespace FlightScanner.WebApi.Validation;

public class PositiveNumberValidation : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not int number)
        {
            return new ValidationResult($"Received input {value} is not number!");
        }

        if (!int.IsPositive(number))
        {
            return new ValidationResult($"Received number {number} should be positive!");
        }

        return ValidationResult.Success;
    }
}
