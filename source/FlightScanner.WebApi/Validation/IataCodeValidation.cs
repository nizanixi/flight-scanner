using System.ComponentModel.DataAnnotations;

namespace FlightScanner.WebApi.Validation;

public class IataCodeValidation : ValidationAttribute
{
    private const int IATA_CODE_LENGTH = 3;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string iataCode)
        {
            return new ValidationResult($"Received input {value} is not text!");
        }

        if (iataCode.Length != IATA_CODE_LENGTH)
        {
            return new ValidationResult($"IATA code has invalid length of {iataCode.Length}. IATA code should have {IATA_CODE_LENGTH} text characters.");
        }

        return ValidationResult.Success;
    }
}
