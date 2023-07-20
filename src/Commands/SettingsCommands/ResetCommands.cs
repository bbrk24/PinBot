using Discord.Interactions;
using PinBot.Models;
using PinBot.Repositories;
using PinBot.Util;

namespace PinBot.Commands;

public partial class SettingsCommand
{
    [Group("reset", "Reset a setting to default")]
    public class ResetCommands : InteractionModuleBase
    {
        private readonly ILogger _logger;
        private readonly IServerSettingsRepository _repository;

        public ResetCommands(
            ILogger<ResetCommands> logger,
            IServerSettingsRepository repository
        )
        {
            _logger = logger;
            _repository = repository;
        }

        [SlashCommand("pinemoji", "The emoji used to pin messages")]
        public async Task ResetPinEmojiAsync()
        {
            try
            {
                await _repository.UpdatePinEmojiAsync(
                    (long)Context.Guild.Id,
                    new ServerSettings().PinEmoji
                ).DeferIfNeeded(Context.Interaction);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Exception occurred while setting pin emoji for {0}",
                    Context.Guild.Id
                );
                await Context.Interaction.RespondOrEditAsync(
                    "An unexpected error occurred while saving.",
                    ephemeral: true
                );
                return;
            }

            await Context.Interaction.RespondOrEditAsync("Pin emoji reset!");
        }

        [SlashCommand("unpinemoji", "The emoji used to unpin messages")]
        public async Task ResetUnpinEmojiAsync()
        {
            try
            {
                await _repository.UpdateUnpinEmojiAsync(
                    (long)Context.Guild.Id,
                    new ServerSettings().UnpinEmoji
                ).DeferIfNeeded(Context.Interaction);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Exception occurred while setting unpin emoji for {0}",
                    Context.Guild.Id
                );
                await Context.Interaction.RespondOrEditAsync(
                    "An unexpected error occurred while saving.",
                    ephemeral: true
                );
                return;
            }

            await Context.Interaction.RespondOrEditAsync("Unpin emoji reset!");
        }

        [SlashCommand("forumsonly", "Whether to only pin in forum threads")]
        public async Task ResetForumsOnlyAsync()
        {
            try
            {
                await _repository.SetForumsOnlyAsync(
                    (long)Context.Guild.Id,
                    new ServerSettings().ForumsOnly
                ).DeferIfNeeded(Context.Interaction);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Exception occurred while setting forums-only mode for {0}",
                    Context.Guild.Id
                );
                await Context.Interaction.RespondOrEditAsync(
                    "An unexpected error occurred while saving.",
                    ephemeral: true
                );
                return;
            }

            await Context.Interaction.RespondOrEditAsync("Forums-only mode reset!");
        }

        [SlashCommand("all", "Reset all settings")]
        public async Task ResetAllAsync()
        {
            try
            {
                await _repository.DeleteSettingsAsync(
                    (long)Context.Guild.Id
                ).DeferIfNeeded(Context.Interaction);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Exception occurred while deleting settings for {0}",
                    Context.Guild.Id
                );
                await Context.Interaction.RespondOrEditAsync(
                    "An unexpected error occurred while saving.",
                    ephemeral: true
                );
                return;
            }

            await Context.Interaction.RespondOrEditAsync("All settings have been reset.");
        }
    }
}
