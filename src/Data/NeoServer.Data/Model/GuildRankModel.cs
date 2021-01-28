using NeoServer.Game.Creatures.Guilds;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Model
{
    public class GuildRankModel
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public virtual GuildModel Guild { get; set; }
    }
}
