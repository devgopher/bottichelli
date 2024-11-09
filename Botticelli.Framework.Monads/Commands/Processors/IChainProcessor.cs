using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Chain processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface IChainProcessor<TCommand> where TCommand : ICommand
{
    public Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(IResult<TCommand> command);
}