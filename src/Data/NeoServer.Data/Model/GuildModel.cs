using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;

namespace NeoServer.Data.Model
{
    public class GuildModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Modt { get; set; }
        public virtual PlayerModel Owner { get; set; }
        public virtual ICollection<GuildMembershipModel> Members { get; set; }
        public virtual ICollection<GuildRankModel> Ranks { get; set; }
    }
}
