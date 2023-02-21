using Discord.WebSocket;

namespace AthenaBot.Extensions
{
    public static class SocketGuildUserExtensions
    {
        public static bool IsOwnerOf(this SocketGuildUser user, SocketGuild guild) {
            if (user == null || guild == null)
                return false;
            return user.Id == guild.OwnerId;
        }

        public static bool IsOwner(this SocketGuild guild, SocketGuildUser user) {
            if (user == null || guild == null)
                return false;
            return guild.OwnerId == user.Id;
        }
    }
}
