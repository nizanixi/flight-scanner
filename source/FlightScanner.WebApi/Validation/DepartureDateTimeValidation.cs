using System.ComponentModel.DataAnnotations;

namespace FlightScanner.WebApi.Validation;

public class DepartureDateTimeValidation : ValidationAttribute
{
    private const int HOURS_OFFSET_FOR_BOARDING = 2;
    private const string DATE_TIME_FORMAT = "MM-DD-YYYY";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string inputDateTimeText)
        {
            return new ValidationResult($"Received input {value} is not date time!");
        }

        if (!DateTime.TryParse(inputDateTimeText, out var inputDateTime))
        {
            return new ValidationResult($"Date time should have this format: {DATE_TIME_FORMAT}!");
        }

        var dateTimeWithOffset = DateTime.Now
            .AddHours(HOURS_OFFSET_FOR_BOARDING);
        if (inputDateTime < dateTimeWithOffset)
        {
            return new ValidationResult($"Received date time {value} is greater than allowed!");
        }

        return ValidationResult.Success;
    }
}
