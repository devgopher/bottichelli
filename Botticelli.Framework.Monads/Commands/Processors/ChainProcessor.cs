using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Shared.ValueObjects;
using LanguageExt;

namespace Botticelli.Framework.Monads.Commands.Processors;

public abstract class ChainProcessor<TCommand> : IChainProcessor<TCommand> where TCommand : ICommand
{
    private readonly ICommandContext _commandContext;

    protected ChainProcessor(ICommandContext commandContext)
    {
        _commandContext = commandContext;
    }

    public async Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(TCommand command)
    {
        try
        {
            await InnerProcessAsync(command);
            
            return SuccessResult<TCommand>.Create(command, _commandContext, new Message());
        }
        catch (Exception ex)
        {
            await InnerErrorProcessAsync(command);
            
            return FailResult<TCommand>.Create(command, _commandContext, ex.Message, new Message());
        }
    }

    protected abstract Task InnerProcessAsync(TCommand command);

    protected abstract Task InnerErrorProcessAsync(TCommand command);
}