using Botticelli.Framework.Monads.Commands.Processors.Multichain;

namespace Botticelli.Framework.Monads.Commands.Context;

/// <summary>
///     Resolved a choice by type
/// </summary>
/// <typeparam name="TNeededType">A needed type</typeparam>
public class ChoiseTypeResolver<TNeededType> : IChoiseResolver
{
    /// <summary>
    ///     Resolves a choise, comparing every argument with a needed type, checks if
    ///     value is transmitted and if so => true, in other cases => false
    /// </summary>
    /// <param name="choise"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool Resolve(IChoise choise)
    {
        var choiceType = choise.GetType();
        var choiceArgsTypes = choise.GetType().GetGenericArguments();
        var choiceArgsCount = choiceArgsTypes.Length;

        if (choiceArgsCount > 4)
            throw new Exception($"Choise type {choiceType} has more than 4 arguments.");

        foreach (var choiceArg in choiceArgsTypes)
            if (choiceArg == typeof(TNeededType))
                return choise.GetValue() is not null;

        return false;
    }
}