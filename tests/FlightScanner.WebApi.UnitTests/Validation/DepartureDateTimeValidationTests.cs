using System.ComponentModel.DataAnnotations;
using FlightScanner.Common.Constants;
using FlightScanner.WebApi.Validation;

namespace FlightScanner.WebApi.UnitTests.Validation;

[TestFixture]
public class DepartureDateTimeValidationTests
{
    [Test]
    public void GetValidationResult_WithNonStringInput_ReturnsValidationError()
    {
        var departureDateTimeValidation = new DepartureDateTimeValidation();
        var nonStringInput = 12345;

        var result = departureDateTimeValidation.GetValidationResult(nonStringInput, new ValidationContext(nonStringInput));

        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result.ErrorMessage, Is.EqualTo("Received input 12345 is not date time!"));
    }

    [Test]
    public void GetValidationResult_WithIncorrectDateTimeFormat_ReturnsValidationError()
    {
        var departureDateTimeValidation = new DepartureDateTimeValidation();
        var dateTimeWithIncorrectFormat = "2024-12-31-99-99";

        var result = departureDateTimeValidation.GetValidationResult(dateTimeWithIncorrectFormat, new ValidationContext(dateTimeWithIncorrectFormat));

        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result.ErrorMessage, Is.EqualTo($"Date time should have this format: {DateTimeConstants.DATE_TIME_FORMAT}!"));
    }

    [Test]
    public void GetValidationResult_WithDateTimeTooSoon_ReturnsValidationError()
    {
        var departureDateTimeValidation = new DepartureDateTimeValidation();
        var tooSoonDateTime = DateTime.Now.AddHours(2).ToString(DateTimeConstants.DATE_TIME_FORMAT);

        var result = departureDateTimeValidation.GetValidationResult(tooSoonDateTime, new ValidationContext(tooSoonDateTime));

        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result.ErrorMessage, Does.StartWith("Received date time"));
    }

    [Test]
    public void GetValidationResult_WithDateTimeExactlyOnBoundaryFlightTime_ReturnsSuccess()
    {
        var departureDateTimeValidation = new DepartureDateTimeValidation();
        var boundaryDateTime = DateTime.Now.AddHours(4).ToString(DateTimeConstants.DATE_TIME_FORMAT);

        var result = departureDateTimeValidation.GetValidationResult(boundaryDateTime, new ValidationContext(boundaryDateTime));

        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }

    [Test]
    public void GetValidationResult_WithDateTimeExactlyOnBoundaryTimingValues_ReturnsSuccess()
    {
        var arrivalDateTimeValidation = new ArrivalDateTimeValidation();
        var boundaryDateTime = $"12-31-{DateTime.Now.Year}-23-59";

        var result = arrivalDateTimeValidation.GetValidationResult(boundaryDateTime, new ValidationContext(boundaryDateTime));

        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }

    [Test]
    public void GetValidationResult_WithValidDateTime_ReturnsSuccess()
    {
        var departureDateTimeValidation = new DepartureDateTimeValidation();
        var validDateTime = DateTime.Now.AddHours(4).ToString(DateTimeConstants.DATE_TIME_FORMAT);

        var result = departureDateTimeValidation.GetValidationResult(validDateTime, new ValidationContext(validDateTime));

        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }
}
