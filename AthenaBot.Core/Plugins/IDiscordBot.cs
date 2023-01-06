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

        bool ValidateCommandRoles(SocketGuild guild, SocketGuildChannel channel, SocketGuildUser user, CommandInfo cmd);
        bool ValidateCommandRoles(SocketGuild guild, SocketGuildChannel channel, SocketGuildUser user, ICommandInfo cmd);
    }
}
