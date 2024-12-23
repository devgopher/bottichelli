using Botticelli.Framework.Commands;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

public interface IChoise : ICommand
{
    /// <summary>
    ///     None value?
    /// </summary>
    public bool None { get; set; }

    /// <summary>
    ///     Validates choice options
    /// </summary>
    public void ValidateOptions();

    /// <summary>
    ///     Get assigned value
    /// </summary>
    /// <returns></returns>
    public object? GetValue();
}