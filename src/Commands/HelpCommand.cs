using Discord.Interactions;

namespace PinBot.Commands;

public class HelpCommand : InteractionModuleBase
{

    [SlashCommand("help", "Explain what the bot does", true)]
    public async Task HelpAsync()
    {
        await Context.Interaction.RespondAsync(@"This bot helps you manage pins in your threads.

React to a message with 📌 to pin it, or with 🚫 to remove a pinned message.
The bot only responds to reactions posted by the thread's OP. Anyone else can pile on 📌, but it won't do anything. It also won't pin messages in non-thread channels.",
            ephemeral: true
        );
    }
}
