namespace Botticelli.Interfaces;

public interface ISendOptionsBuilder<T>
{
    ISendOptionsBuilder<T> Create(params object[]? args);
    ISendOptionsBuilder<T> Set(Func<T?, T> func);
    T? Build();
}