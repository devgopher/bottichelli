using System.Reflection;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;

namespace TelegramMonadsBasedBot.Commands.Processors;

public class OutputCommandProcessor<TReplyMarkup, TCommand> : ChainProcessor<TCommand> 
    where TReplyMarkup : class
    where TCommand : IChainCommand
{
    private readonly SendOptionsBuilder<TReplyMarkup> _options;

    public OutputCommandProcessor(ILogger<OutputCommandProcessor<TReplyMarkup, TCommand>> logger,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser)
        : base(logger)
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);

        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }
    
    protected override async Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token)
    {
        var message = GetMessage(stepResult.Command);
        var outputMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = $"Result:{GetArgs(stepResult.Command)}"
            }
        };

        await Bot.SendMessageAsync(outputMessageRequest, _options, token);
    }

    protected override Task InnerErrorProcessAsync(IResult<TCommand> command, CancellationToken token) => throw new NotImplementedException();
}