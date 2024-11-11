using System.Reflection;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;

namespace TelegramMonadsBasedBot.Commands.Processors;

public class StartCommandPromptProcessor<TReplyMarkup> : ChainProcessor<StartCommand>
    where TReplyMarkup : class
{
    private readonly SendOptionsBuilder<TReplyMarkup> _options;
    
    public StartCommandPromptProcessor(ILogger<StartCommandPromptProcessor<TReplyMarkup>> logger,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser)
    : base(new CommandContext(), logger)
    { 
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);

        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }


    protected override async Task InnerProcessAsync(IResult<StartCommand> stepResult, CancellationToken token)
    {
        var message = GetMessage(stepResult.Command);
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "Сhain step 2: Enjoy!"
            }
        };

        await Bot.SendMessageAsync(greetingMessageRequest, _options, token);
    }

    protected override Task InnerErrorProcessAsync(IResult<StartCommand> command, CancellationToken token) =>
        throw new NotImplementedException();
}