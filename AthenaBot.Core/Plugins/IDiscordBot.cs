using AthenaBot.Commands;
using AthenaBot.Configuration;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System.ComponentModel;

namespace AthenaBot.Plugins
{
    public interface IDiscordBot : INotifyPropertyChanged
    {
        Directories Directories { get; }
        DiscordBotConfig Config { get; }
        DiscordSocketClient Client { get; }
        IDiscordBotPluginHost Plugins { get; }

        ServerConfig FindConfig(ulong guildId);

        bool ValidateCommandRoles(DiscordBotCommandContext context, CommandInfo cmd);
        bool ValidateCommandRoles(DiscordBotInteractionContext context, ICommandInfo cmd);
    }
}
