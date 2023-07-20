using Discord.Interactions;
using Discord;
using PinBot.Attributes;
using PinBot.Util;
using PinBot.Repositories;
using PinBot.Models;

namespace PinBot.Commands;

[Group("config", "See or update configuration for this server.")]
[Moderator(GuildPermission.ManageMessages)]
public partial class SettingsCommand : InteractionModuleBase
{
    private readonly ILogger _logger;
    private readonly IServerSettingsRepository _repository;

    public SettingsCommand(
        ILogger<SettingsCommand> logger,
        IServerSettingsRepository repository
    )
    {
        _logger = logger;
        _repository = repository;
    }

    [SlashCommand("get", "Show the current config")]
    public async Task GetSettingsAsync(
        [Summary("hidden", "Show the result only to you")] bool ephemeral = false
    )
    {
        ServerSettings settings;
        try
        {
            settings = await _repository.GetSettingsAsync((long)Context.Guild.Id)
                .DeferIfNeeded(Context.Interaction, ephemeral: ephemeral);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Exception while fetching settings for {0}",
                Context.Guild.Id
            );
            await Context.Interaction.RespondOrEditAsync(
                "Could not access server settings.",
                ephemeral: true
            );
            return;
        }

        await Context.Interaction.RespondOrEditAsync(
            settings.ToString(),
            ephemeral: ephemeral
        );
    }
}
