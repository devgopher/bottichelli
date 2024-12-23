using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

public class MultiChainRunner<TCommand>(List<IMultiChainProcessor<IChoise>> chain, ILogger<MultiChainRunner<TCommand>> logger)
    where TCommand : IChainCommand
{
    public async Task<EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>>> Run(TCommand command)
    {
        var output = EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>>
            .Right(SuccessResult<TCommand>.Create(command));

        logger.LogInformation("Chain processing for {TCommand} start...", typeof(TCommand).Name);
        foreach (var tc in chain)
        {
            logger.LogInformation("Chain processor {tc} for {TCommand} start...", tc.GetType().Name,
                typeof(TCommand).Name);

            await output.BiIter(r => tc.Process(r),
                l => logger.LogInformation("Chain processor {tc}  for {TCommand} finished: fail!", tc.GetType().Name,
                    typeof(TCommand).Name));

            logger.LogInformation("Chain processor {tc} for {TCommand} finished: success", tc.GetType().Name,
                typeof(TCommand).Name);
        }

        logger.LogInformation("Chain processing for {TCommand} finished: success", typeof(TCommand).Name);

        return output;
    }
}