using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace Botticelli.Shared.Validation;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator() 
    {
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.Uid).NotEmpty();
    }
}