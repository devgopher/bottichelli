using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class OutputCommandProcessor<TReplyMarkup, TCommand> : ChainProcessor<TCommand> 
    where TReplyMarkup : class
    where TCommand : IChainCommand
{
    private readonly SendOptionsBuilder<TReplyMarkup> _options;

    public OutputCommandProcessor(ILogger<OutputCommandProcessor<TReplyMarkup, TCommand>> logger,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser)
        : base(logger) =>
        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder();

    protected override async Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token)
    {
        var message = GetMessage(stepResult.Command);
        var outputMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message!.ChatIds,
                Body = GetArgs(stepResult.Command)
            }
        };

        await Bot.SendMessageAsync(outputMessageRequest, _options, token);
    }

    protected override Task InnerErrorProcessAsync(FailResult<TCommand> stepResult, CancellationToken token) => throw new NotImplementedException();
}