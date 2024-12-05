using System.ComponentModel.DataAnnotations;
using FlightScanner.Common.Enumerations;

namespace FlightScanner.WebApi.Validation;

/// <summary>
/// Validates according to ISO 4217 standard.
/// </summary>
public class CurrencyValidation : ValidationAttribute
{
    private const int CURRENCY_CODE_LENGTH = 3;
    private static readonly string[] s_supportedCurrencies = Enum.GetNames<Currency>();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string currencyCode)
        {
            return new ValidationResult($"Received input {value} is not text!");
        }

        if (currencyCode.Length != CURRENCY_CODE_LENGTH)
        {
            return new ValidationResult($"Currency code has invalid length of {currencyCode.Length}. Currency code should have {CURRENCY_CODE_LENGTH} text characters.");
        }

        var isInputCurrencySupported = s_supportedCurrencies
            .Any(currency => string.Equals(currency, currencyCode, StringComparison.OrdinalIgnoreCase));
        if (!isInputCurrencySupported)
        {
            return new ValidationResult($"Received unsupported currency: {currencyCode}!" +
                $"\nSupported currencies: {Currency.USD}, {Currency.EUR}, {Currency.HRK}.");
        }

        return ValidationResult.Success;
    }
}
