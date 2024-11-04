using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;

namespace Botticelli.Framework.Monads.Commands.Processors;

public interface IChainProcessor<TCommand> where TCommand : ICommand
{
    public Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(TCommand command);
}