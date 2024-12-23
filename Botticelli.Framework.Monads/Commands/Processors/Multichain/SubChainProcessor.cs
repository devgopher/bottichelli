using Botticelli.Framework.Monads.Commands.Context;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

public class SubChainProcessor<TCommand, TChoise> : TransformArgumentsProcessor<TCommand, TChoise> 
    where TCommand : IChainCommand
    where TChoise : IChoise
{
    public SubChainProcessor(ILogger<TransformArgumentsProcessor<TCommand, TChoise>> logger) : base(logger)
    {
    }
}