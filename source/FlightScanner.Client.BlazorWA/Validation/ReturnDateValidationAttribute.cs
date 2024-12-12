using System.ComponentModel.DataAnnotations;

namespace FlightScanner.Client.BlazorWA.Validation;

public class ReturnDateValidationAttribute : ValidationAttribute
{
    private readonly string _departureDatePropertyName;

    public ReturnDateValidationAttribute(string departureDatePropertyName)
    {
        _departureDatePropertyName = departureDatePropertyName;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime returnDate)
        {
            throw new ArgumentException("Input value is not DateTime property");
        }

        var departureDateProperty = validationContext.ObjectType.GetProperty(_departureDatePropertyName);
        if (departureDateProperty == null)
        {
            throw new ArgumentException($"Property with name '{_departureDatePropertyName}' not found");
        }

        var departureDate = departureDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;
        if (departureDate.HasValue && returnDate.Date < departureDate.Value.Date)
        {
            return new ValidationResult(ErrorMessage ?? "Return date cannot be earlier than departure date.");
        }

        return ValidationResult.Success!;
    }
}
