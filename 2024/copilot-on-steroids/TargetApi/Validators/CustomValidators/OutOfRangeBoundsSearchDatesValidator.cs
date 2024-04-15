using FluentValidation;
using FluentValidation.Validators;

namespace FunctionCalling.Validators.CustomValidators;

public class OutOfRangeBoundsSearchDatesValidator<T> : PropertyValidator<T, DateTime?>
{
    private readonly DateTime _minDate = new DateTime(2020, 1, 1);
    public override bool IsValid(ValidationContext<T> context, DateTime? value)
    {
        if (value is null)
        {
            return true;
        }  
        if (value <= _minDate)
        {
            return false;
        }
        return true;
    }

    public override string Name => "OutOfRangeBoundsSearchDatesValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => new ValidationErrorInfo { ErrorCode = $"Minimum value for dates in search must be greater than {_minDate:yyyy MM dd}"
            ,  AssistantAction = $"ask the user to provide a date whose value is greater than {_minDate:yyyy MM dd}" }.ToJson();
}

public static class OutOfRangeBoundsSearchDatesValidatorExtensions
{
    public static IRuleBuilderOptions<T, DateTime?> OutOfRangeBoundsSearchDatesValidator<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new OutOfRangeBoundsSearchDatesValidator<T>());
    }
}