using System.ComponentModel.DataAnnotations;
using FlightScanner.Common.Constants;

namespace FlightScanner.WebApi.Validation;

public class IataCodeValidation : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string iataCode)
        {
            return new ValidationResult($"Received input {value} is not text!");
        }

        if (iataCode.Length != IataCodeConstants.IATA_CODE_LENGTH)
        {
            return new ValidationResult($"IATA code has invalid length of {iataCode.Length}. IATA code should have {IataCodeConstants.IATA_CODE_LENGTH} text characters.");
        }

        return ValidationResult.Success;
    }
}
