using Botticelli.Framework.Exceptions;
using Botticelli.Interfaces;

namespace Botticelli.Framework.SendOptions;

/// <summary>
///     Additional options for sending messages for partial messenger
///     (for example you can use InlineKeyboardMarkup  as T)
/// </summary>
/// <typeparam name="T"></typeparam>
public class SendOptionsBuilder<T> : ISendOptionsBuilder<T> where T : class
{
    private T _innerObject;

    private SendOptionsBuilder()
    {
    }

    protected SendOptionsBuilder(T innerObject) => _innerObject = innerObject;

    public ISendOptionsBuilder<T> Create(params object[] args)
    {
        if (_innerObject != default) throw new BotException($"You shouldn't use {nameof(Create)}() method twice!");

        var constructors = typeof(T)
            .GetConstructors()
            .Where(c => c.IsPublic)
            .ToArray();

        // no params? ok => let's seek a parameterless constructor!
        if ((args == null || !args.Any()) && constructors.Any(c => !c.GetParameters().Any()))
        {
            _innerObject = Activator.CreateInstance<T>();

            return this;
        }

        // Let's see if we can process parameter set and put it to a constructor|initializer
        foreach (var c in constructors)
        {
            // c.CallingConvention = 
        }


        return this;
    }

    public ISendOptionsBuilder<T> Set(Func<T, T> func)
    {
        func?.Invoke(_innerObject);

        return this;
    }

    public T Build() => _innerObject;

    public static SendOptionsBuilder<T> CreateBuilder() => new();

    public static SendOptionsBuilder<T> CreateBuilder(T input) => new(input);
}