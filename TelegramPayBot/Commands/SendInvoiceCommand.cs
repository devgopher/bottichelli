using Botticelli.Framework.Commands;

namespace TelegramPayBot.Commands;

public class SendInvoiceCommand : ICommand
{
    public Guid Id { get; }
}