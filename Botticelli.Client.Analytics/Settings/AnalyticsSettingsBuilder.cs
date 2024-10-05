namespace Botticelli.Client.Analytics.Settings;

public class AnalyticsSettingsBuilder<T>
        where T : AnalyticsSettings, new()
{
    private T _settings = new();

    public void Set(T settings) => _settings = settings;
    
    public AnalyticsSettingsBuilder<T> Set(Action<T> func)
    {
        func(_settings);

        return this;
    }

    public T Build() => _settings;
}