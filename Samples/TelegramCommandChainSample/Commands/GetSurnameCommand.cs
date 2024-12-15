using Botticelli.Framework.Commands;

namespace TelegramCommandChainSample.Commands;

public class GetSurnameCommand : ICommand
{
    public Guid Id { get; }
}