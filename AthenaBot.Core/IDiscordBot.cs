using AthenaBot.Commands;
using AthenaBot.Configuration;
using AthenaBot.Interactions;
using AthenaBot.Plugins;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System.ComponentModel;

namespace AthenaBot
{
    public interface IDiscordBot : INotifyPropertyChanged
    {
        bool IsReady { get; }
        bool IsConnected { get; }

        Directories Directories { get; }
        DiscordBotConfig Config { get; }
        DiscordSocketClient Client { get; }
        IAthenaBotPluginHost Plugins { get; }

        void SaveConfig();
        Task SaveConfigAsync();

        bool ValidateCommandRoles(AthenaCommandContext context, CommandInfo cmd);
        bool ValidateCommandRoles(AthenaInteractionContext context, ICommandInfo cmd);
    }
}
