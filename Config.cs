using System.Text.Json;

public sealed class Config
{
    public string Token { get; set; } = "";
    public ulong SysId { get; set; }

    public static Config Load(string path = "appsettings.json")
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Fichier de config introuvable: {path}");

        var json = File.ReadAllText(path);
        var cfg = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (cfg is null || string.IsNullOrWhiteSpace(cfg.Token))
            throw new Exception("Config invalide: Token manquant dans appsettings.json.");

        if (cfg.SysId == 0)
            throw new Exception("Config invalide: SysId manquant (ou 0) dans appsettings.json.");

        return cfg;
    }
}
