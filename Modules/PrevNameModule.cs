using Discord;
using Discord.Interactions;

public sealed class PrevNameModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly Db _db;
    private readonly Config _cfg;

    public PrevNameModule(Db db, Config cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [SlashCommand("prevname", "Permet de voir les anciens pseudo (global) de l'utilisateur")]
    public async Task PrevNameAsync(IUser? user = null)
    {
        user ??= Context.User;

        var history = _db.GetHistory(user.Id, 25);

        if (history.Count == 0)
        {
            var eb = Embeds.Base("🕓 Historique global")
                .WithDescription($"Aucun historique trouvé pour {user.Mention}.")
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

            await RespondAsync(embed: eb.Build(), ephemeral: false);
            return;
        }

        var header = $"Historique global de {user.Mention}\n" +
                     $"**{history.Count}** changement(s) récent(s) affiché(s) (max 25).";

        var ebHist = Embeds.Base("🕓 Historique global")
            .WithDescription(header)
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

        int i = 1;
        foreach (var h in history)
        {
            // Timestamp Discord: <t:unix:R> (relatif) + <t:unix:f> (date complète)
            var rel = $"<t:{h.ChangedAt}:R>";
            var full = $"<t:{h.ChangedAt}:f>";

            ebHist.AddField(
                $"#{i} • {rel}",
                $"`{h.OldName}` → `{h.NewName}`\n📅 {full}",
                inline: false
            );
            i++;
        }

        await RespondAsync(embed: ebHist.Build(), ephemeral: false);
    }

    [SlashCommand("prevname-clear", "Permet de clear les pseudos de la personne dans la DB (SYS uniquement)")]
    public async Task PrevNameClearAsync(IUser user)
    {
        if (Context.User.Id != _cfg.SysId)
        {
            var eb = Embeds.Base("⛔ Accès refusé")
                .WithDescription("Seul le **SYS** peut utiliser cette commande.")
                .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());

            await RespondAsync(embed: eb.Build(), ephemeral: true);
            return;
        }

        var n = _db.ClearHistory(user.Id);

        var ok = Embeds.Base("✅ Historique supprimé")
            .WithDescription($"Historique supprimé pour {user.Mention}.\n**{n}** entrée(s) supprimée(s).")
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

        await RespondAsync(embed: ok.Build(), ephemeral: false);
    }

    [SlashCommand("help", "Affiche les commandes disponibles")]
    public async Task HelpAsync()
    {
        var eb = Embeds.Base("📌 Commandes")
            .WithDescription("Bot **PrevName**")
            .AddField("🕓 /prevname <utilisateur>", "Voir l’historique global d’un utilisateur.", false)
            .AddField("🧹 /prevname-clear <utilisateur>", "Supprimer l’historique d’un utilisateur.", false);
        await RespondAsync(embed: eb.Build(), ephemeral: false);
    }
}