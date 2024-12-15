namespace Botticelli.Client.Analytics.Settings;

public class AnalyticsClientSettingsBuilder<T>
        where T : AnalyticsClientSettings, new()
{
    private T _settings = new();

    public void Set(T settings) => _settings = settings;
    
    public AnalyticsClientSettingsBuilder<T> Set(Action<T> func)
    {
        func(_settings);

        return this;
    }

    public T Build() => _settings;
}