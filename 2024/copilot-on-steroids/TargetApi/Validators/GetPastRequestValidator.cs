using FluentValidation;
using FunctionCalling.Controllers;
using FunctionCalling.Validators.CustomValidators;

namespace FunctionCalling.Validators;

public class GetPastRequestValidator : AbstractValidator<GetPastRequest>
{
    private const int MinLengthLocationName = 3;

    public GetPastRequestValidator()
    {
        RuleFor(x => x.Location).NotEmpty().WithMessage(d => new ValidationErrorInfo
        {
            ErrorCode = "invalid location",
            AssistantAction = "ask the user to provide a valid location"
        }.ToJson());
        RuleFor(x => x.Location).Length(MinLengthLocationName, 100).
            WithMessage(d => new ValidationErrorInfo
            {
                ErrorCode = "{PropertyName} value is invalid",
                AssistantAction = "reply to the user with these exact words: '{PropertyName} must have a value of at least " + MinLengthLocationName + " characters"
            }.ToJson());
        RuleFor(x => x.Date).Must(d=> { 
            if(d==DateTime.MinValue)
            {
                return false;
            }
            if (d >= DateTime.Now)
            {
                return false;
            }
            return true;
        }).WithMessage(d => new ValidationErrorInfo
        {
            ErrorCode = "invalid date",
            AssistantAction = $"ask the user a date that is less then {DateTime.Now:yyyy-MM-dd}"
        }.ToJson());
        RuleFor(x => x.Location).Length(MinLengthLocationName, 100).
            WithMessage(d => new ValidationErrorInfo
            {
                ErrorCode = "{PropertyName} value is invalid",
                AssistantAction = "reply to the user with these exact words: '{PropertyName} must have a value of at least " + MinLengthLocationName + " characters"
            }.ToJson());
    }
}