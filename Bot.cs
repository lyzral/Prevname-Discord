using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public sealed class Bot
{
    private bool _readyOnce = false;

    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;

    private readonly Db _db;
    private readonly Config _cfg;

    public Bot()
    {
        _cfg = Config.Load();
        _db = new Db();

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
            LogGatewayIntentWarnings = false
        });

        _interactions = new InteractionService(_client.Rest);

        _services = new ServiceCollection()
            .AddSingleton(_cfg)
            .AddSingleton(_db)
            .AddSingleton(_client)
            .AddSingleton(_interactions)
            .BuildServiceProvider();
    }

    public async Task RunAsync()
    {
        _client.Log += msg => { Console.WriteLine(msg.ToString()); return Task.CompletedTask; };
        _interactions.Log += msg => { Console.WriteLine(msg.ToString()); return Task.CompletedTask; };

        _client.Ready += OnReadyAsync;
        _client.InteractionCreated += OnInteractionCreatedAsync;

        // Global only: username / global display name
        _client.UserUpdated += OnUserUpdatedAsync;

        await _client.LoginAsync(TokenType.Bot, _cfg.Token);
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task OnReadyAsync()
    {
        // évite double exécution (reconnect, etc.)
        if (_readyOnce) return;
        _readyOnce = true;

        await _interactions.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

        // Déploiement GLOBAL (Discord peut mettre un peu de temps à propager)
        // deleteMissing=true permet de nettoyer les anciennes commandes de CETTE application.
        await _interactions.RegisterCommandsGloballyAsync(deleteMissing: true);

        Console.WriteLine("✅ Commandes déployées en GLOBAL");
        Console.WriteLine($"✅ Connecté en tant que {_client.CurrentUser}");
    }

    private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            try
            {
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    var original = await interaction.GetOriginalResponseAsync();
                    await original.DeleteAsync();
                }
            }
            catch { /* ignore */ }
        }
    }

    private Task OnUserUpdatedAsync(SocketUser before, SocketUser after)
    {
        // Global only: GlobalName (si dispo) sinon Username.
        var beforeName = !string.IsNullOrWhiteSpace(before.GlobalName) ? before.GlobalName : before.Username;
        var afterName  = !string.IsNullOrWhiteSpace(after.GlobalName) ? after.GlobalName : after.Username;

        if (string.Equals(beforeName, afterName, StringComparison.Ordinal))
            return Task.CompletedTask;

        var unix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _db.AddHistory(after.Id, beforeName, afterName, unix);

        return Task.CompletedTask;
    }
}
