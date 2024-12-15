using Botticelli.Framework.Commands;

namespace TelegramCommandChainSample.Commands;

public class GetNameCommand : ICommand
{
    public Guid Id { get; }
}