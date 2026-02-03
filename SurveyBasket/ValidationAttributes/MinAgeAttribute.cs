using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MinAgeAttribute(int minAge) : ValidationAttribute
{
    private readonly int _minAge = minAge;

    //public override bool IsValid(object? value)
    //{
    //    if(value is not null)
    //    {
    //        var date = (DateTime)value;
    //        if(DateTime.Today < date.AddYears(_minAge))
    //        {
    //            return false;
    //        }
    //    }
    //    return true;

    //}

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not null)
        {
            var date = (DateTime)value;
            if (DateTime.Today < date.AddYears(_minAge))
            {
                return new ValidationResult(ErrorMessage ?? $"The field {validationContext.DisplayName} must indicate an age of at least {_minAge} years.");
            }
        }
        return ValidationResult.Success;
    }
}