﻿using FlightScanner.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FlightScanner.WebApi.Validation;

public class ArrivalDateTimeValidation : ValidationAttribute
{
    private const int OFFSET_FOR_BOARDING_IN_HOURS = 2;
    private const int OFFSET_FOR_NEXT_FLIGHT_RESERVATION_IN_HOURS = 1;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string inputDateTimeText)
        {
            return new ValidationResult($"Received input {value} is not date time!");
        }

        if (!DateTime.TryParseExact(inputDateTimeText, DateTimeConstants.DATE_TIME_FORMAT, null, DateTimeStyles.None, out var inputDateTime))
        {
            return new ValidationResult($"Date time should have this format: {DateTimeConstants.DATE_TIME_FORMAT}!");
        }

        var differenceInHours = inputDateTime.Subtract(DateTime.Now).TotalHours;
        if (differenceInHours < OFFSET_FOR_NEXT_FLIGHT_RESERVATION_IN_HOURS + OFFSET_FOR_BOARDING_IN_HOURS)
        {
            return new ValidationResult($"Received date time {value} is greater than allowed!");
        }

        return ValidationResult.Success;
    }
}
