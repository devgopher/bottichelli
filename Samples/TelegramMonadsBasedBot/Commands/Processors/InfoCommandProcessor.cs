using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace TelegramMonadsBasedBot.Commands.Processors;

public class InfoCommandProcessor<TReplyMarkup> : CommandProcessor<InfoCommand> where TReplyMarkup : class
{
    private readonly SendOptionsBuilder<TReplyMarkup> _options;
    
    public InfoCommandProcessor(ILogger<InfoCommandProcessor<TReplyMarkup>> logger, 
        ICommandValidator<InfoCommand> validator, 
        MetricsProcessor metricsProcessor,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser) : base(logger, validator, metricsProcessor)
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);
        
        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }

    protected override Task InnerProcessContact(Message message, string args, CancellationToken token) => throw new NotImplementedException();

    protected override Task InnerProcessPoll(Message message, string args, CancellationToken token) => throw new NotImplementedException();

    protected override Task InnerProcessLocation(Message message, string args, CancellationToken token) => throw new NotImplementedException();

    protected override async Task InnerProcess(Message message, string args, CancellationToken token)
    {
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "This is a test bot.\nEnjoy!"
            }
        };

        await Bot?.SendMessageAsync(greetingMessageRequest, _options, token)!; // TODO: think about Bot mocks
    }
}