using Discord.Interactions;
using PinBot.Repositories;

namespace PinBot.Commands;

public class HelpCommand : InteractionModuleBase
{
    private const string _helpFormat = @"This bot helps you manage pins in your threads.

React to a message with {0} to pin it, or with {1} to remove a pinned message.
The bot only responds to reactions posted by the thread's OP. Anyone else can pile on {0}, but it won't do anything. It also won't pin messages in non-thread channels.";

    private readonly IServerSettingsRepository _serverSettingsRepository;

    public HelpCommand(
        IServerSettingsRepository serverSettingsRepository
    )
    {
        _serverSettingsRepository = serverSettingsRepository;
    }

    [SlashCommand("help", "Explain what the bot does", true)]
    public async Task HelpAsync()
    {
        var settings = await _serverSettingsRepository
            .GetSettingsAsync((long)(Context.Guild?.Id ?? 0));

        await Context.Interaction.RespondAsync(
            string.Format(_helpFormat, settings.PinEmoji, settings.UnpinEmoji),
            ephemeral: true
        );
    }
}
