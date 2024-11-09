using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Shared.ValueObjects;
using LanguageExt;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class ChainBuilder<TCommand> where TCommand : ICommand
{
    
    
    public ChainBuilder<TCommand> Add(ChainBuilder<TCommand> processor)
    {
        
    }

    public ChainRunner<TCommand> Build()
    {
        
        
        
    }
}

public class ChainRunner<TCommand> where TCommand : ICommand
{
    private readonly List<ChainProcessor<TCommand>> _chain;

    public ChainRunner(List<ChainProcessor<TCommand>> chain)
    {
        _chain = chain;
    }

    public async Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Run(TCommand command)
    {
        var output = new Either<FailResult<TCommand>, SuccessResult<TCommand>>();
        
        foreach (var tc in _chain)
        {
            output = await tc.Process(output);

            if (!output.IsSuccess) return output;
        }
        
        return output;;
    }
}

/// <summary>
///     Chain processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// TODO: add logging!
public abstract class ChainProcessor<TCommand>(ICommandContext commandContext) : IChainProcessor<TCommand>
        where TCommand : ICommand
{
    public async Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(IResult<TCommand> command)
    {
        try
        {
            if (command.IsSuccess)
            {
                await InnerProcessAsync(command);

                return SuccessResult<TCommand>.Create(command.Command, commandContext, command.Message));
            }

            return FailResult<TCommand>.Create(command.Command,
                                               commandContext,
                                               string.Empty,
                                               command.Message);
        }
        catch (Exception ex)
        {
            await InnerErrorProcessAsync(command);

            return FailResult<TCommand>.Create(command.Command,
                                               commandContext,
                                               ex.Message,
                                               command.Message);
        }
    }

    protected abstract Task InnerProcessAsync(IResult<TCommand> command);

    protected abstract Task InnerErrorProcessAsync(IResult<TCommand> command);
}