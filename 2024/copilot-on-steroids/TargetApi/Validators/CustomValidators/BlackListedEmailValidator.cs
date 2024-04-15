using FluentValidation;
using FluentValidation.Validators;

namespace FunctionCalling.Validators.CustomValidators;

public class BlackListedEmailValidator<T> : PropertyValidator<T, string>
{
    private readonly List<string> _blackListedValues = new List<string> { "@example.com" };
    public override bool IsValid(ValidationContext<T> context, string value)
    {
        foreach (var token in _blackListedValues)
        {
            if (value.Contains(token))
            {
                return false;
            }
        }
        return true;
    }

    public override string Name => "BlackListedEMailValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => new ValidationErrorInfo { ErrorCode = "invalid email domain", AssistantAction = "ask the user to provide an email address from a different domain" }.ToJson();
}

public static class BlackListedEmailValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> IsNotAChatGptGeneratedEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new BlackListedEmailValidator<T>());
    }
}