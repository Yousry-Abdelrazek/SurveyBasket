using System.ComponentModel.DataAnnotations;
namespace SurveyBasket.ValidationAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MinAgeAttribute(int minAge) : ValidationAttribute
{
    private int _minAge = minAge;
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not null)
        {
            var date = (DateTime)value;
            if (DateTime.Now.Year - date.Year < _minAge)
            {
                return new ValidationResult($"Age must be at least {_minAge} years.");
            }
        }
        return ValidationResult.Success;
    }
}
