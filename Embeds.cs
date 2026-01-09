using Discord;

public static class Embeds
{
    // Couleur "clean" proche du thème sombre Discord
    public static readonly Color Accent = new Color(0x2B, 0x2D, 0x31);

    public static EmbedBuilder Base(string title)
        => new EmbedBuilder()
            .WithTitle(title)
            .WithColor(Accent)
            .WithCurrentTimestamp()
            .WithFooter("Dev Lyzral");
}
