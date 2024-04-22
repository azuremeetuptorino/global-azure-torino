using FluentValidation;
using FunctionCalling.Controllers.Dtos;
using FunctionCalling.Validators.CustomValidators;

namespace FunctionCalling.Validators;

public class GetQuotesByUserRequestValidator : AbstractValidator<GetQuotesByUserRequest>
{
    public GetQuotesByUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(d => new ValidationErrorInfo { ErrorCode = "invalid email", AssistantAction = "ask the user to provide a valid email address" }.ToJson());
        RuleFor(x => x.Email).EmailAddress().WithMessage(d => new ValidationErrorInfo { ErrorCode = "invalid email", AssistantAction = "ask the user to provide a valid email address" }.ToJson());
        RuleFor(x => x.Email).IsNotAChatGptGeneratedEmail().WithMessage(d => new ValidationErrorInfo { ErrorCode = "invalid domain in email", AssistantAction = "ask the user to provide a valid email address" }.ToJson());
        RuleFor(x => x.DateFrom).OutOfRangeBoundsSearchDatesValidator();
        RuleFor(x => x.DateTo).OutOfRangeBoundsSearchDatesValidator();
    }
}