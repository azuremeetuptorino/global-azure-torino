using FluentValidation;
using FunctionCalling.Controllers;
using FunctionCalling.Validators.CustomValidators;

namespace FunctionCalling.Validators;

public class GetCurrentWeatherRequestValidator : AbstractValidator<GetCurrentWeatherRequest>
{
    private const int MinLengthLocationName = 3;

    public GetCurrentWeatherRequestValidator()
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
    }
}