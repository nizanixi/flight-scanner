using System.ComponentModel.DataAnnotations;
using FlightScanner.WebApi.Validation;

namespace FlightScanner.WebApi.UnitTests.Validation;

[TestFixture]
public class CurrencyValidationTests
{
    [Test]
    public void GetValidationResult_WithUnsupportedCurrencyCode_ReturnsValidationError()
    {
        var currencyValidation = new CurrencyValidation();
        var unsupportedCurrency = "GBP";

        var result = currencyValidation.GetValidationResult(unsupportedCurrency, new ValidationContext(unsupportedCurrency));

        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result.ErrorMessage, Is.EqualTo("Received unsupported currency: GBP!\nSupported currencies: USD, EUR, HRK."));
    }

    [Test]
    public void GetValidationResult_WithIncorrectLengthCurrencyCode_ReturnsValidationError()
    {
        var currencyValidation = new CurrencyValidation();
        var incorrectLengthCurrency = "EURO";

        var result = currencyValidation.GetValidationResult(incorrectLengthCurrency, new ValidationContext(incorrectLengthCurrency));

        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result.ErrorMessage, Is.EqualTo("Currency code has invalid length of 4. Currency code should have 3 text characters."));
    }

    [Test]
    public void GetValidationResult_WithNonStringInput_ReturnsValidationError()
    {
        var currencyValidation = new CurrencyValidation();
        var nonStringInput = 123;

        var result = currencyValidation.GetValidationResult(nonStringInput, new ValidationContext(nonStringInput));

        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result.ErrorMessage, Is.EqualTo("Received input 123 is not text!"));
    }

    [Test]
    public void GetValidationResult_WithCaseInsensitiveCurrencyCode_ReturnsSuccess()
    {
        var currencyValidation = new CurrencyValidation();
        var validCurrency = "usd";

        var result = currencyValidation.GetValidationResult(validCurrency, new ValidationContext(validCurrency));

        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }

    [Test]
    public void GetValidationResult_WithValidCurrencyCode_ReturnsSuccess()
    {
        var currencyValidation = new CurrencyValidation();
        var validCurrency = "USD";

        var result = currencyValidation.GetValidationResult(validCurrency, new ValidationContext(validCurrency));

        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }
}
