using NeoServer.Game.Contracts.Chats;
using System.Linq;

namespace NeoServer.Server.Jobs.Creatures
{
    public class ChatUserCleanupJob
    {
        public static void Execute(IChatChannel channel)
        {
            var removedUsers = channel.Users.Where(x => x.Removed && !x.IsMuted);

            foreach (var user in removedUsers)
            {
                channel.RemoveUser(user.Player);
            }

        }
    }
}
