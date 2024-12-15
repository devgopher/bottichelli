using Botticelli.AI.Message;
using FluentValidation;

namespace Botticelli.AI.Validation;

public class AiMessageValidator : AbstractValidator<AiMessage>
{
    public AiMessageValidator()
    {
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.Uid).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.Instruction).NotEmpty();
    }
}