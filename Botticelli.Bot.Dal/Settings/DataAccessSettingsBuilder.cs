namespace Botticelli.Bot.Data.Settings;

public class DataAccessSettingsBuilder<T>
        where T : IDataAccessSettings, new()
{
    private T _settings = new();

    public void Set(T settings) => _settings = settings;

    public DataAccessSettingsBuilder<T> Set(Action<T> func)
    {
        func(_settings);

        return this;
    }

    public T Build() => _settings;
}