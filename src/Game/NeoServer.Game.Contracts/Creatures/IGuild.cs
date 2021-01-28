using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IGuild
    {
        ushort Id { get; set; }
        string Name { get; set; }
        HashSet<IGuildMember> GuildMembers { get; set; }
    }
    public interface IGuildMember
    {

    }
}
