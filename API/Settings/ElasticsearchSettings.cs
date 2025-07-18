namespace API.Settings;

public class ElasticSearchSettings
{
    public string Url { get; set; } = string.Empty;
    public string DefaultIndex { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableDebugMode { get; set; } = false;
}
