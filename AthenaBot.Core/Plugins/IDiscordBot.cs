using AthenaBot.Commands;
using AthenaBot.Configuration;
using AthenaBot.Interactions;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System.ComponentModel;

namespace AthenaBot.Plugins
{
    public interface IDiscordBot : INotifyPropertyChanged
    {
        bool IsReady { get; }
        bool IsConnected { get; }

        Directories Directories { get; }
        DiscordBotConfig Config { get; }
        DiscordSocketClient Client { get; }
        IDiscordBotPluginHost Plugins { get; }

        ServerConfig FindConfig(ulong guildId);

        bool ValidateCommandRoles(AthenaCommandContext context, CommandInfo cmd);
        bool ValidateCommandRoles(AthenaInteractionContext context, ICommandInfo cmd);
    }
}
