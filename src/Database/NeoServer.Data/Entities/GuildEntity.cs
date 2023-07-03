using System;
using System.Collections.Generic;

namespace NeoServer.Data.Entities;

public class GuildEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreationDate { get; set; }
    public string Modt { get; set; }
    public virtual PlayerEntity Owner { get; set; }
    public virtual ICollection<GuildMembershipEntity> Members { get; set; }
    public virtual ICollection<GuildRankEntity> Ranks { get; set; }
}