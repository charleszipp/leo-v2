namespace Leo.Core.Settings
{
    public interface ISettingsProvider
    {
        T Get<T>(string prefix) where T : new();

        string Get(string key, string defaultValue);

        bool GetAsBool(string key, bool defaultValue);

        decimal GetAsDecimal(string key, decimal defaultValue);

        int GetAsInt(string key, int defaultValue);
    }
}