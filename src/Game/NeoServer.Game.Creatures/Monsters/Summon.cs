using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Model.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Monsters
{
    public class Summon : Monster
    {
        public override bool IsSummon => true;
        public new event Die OnKilled;
        public Summon(IMonsterType type) : base(type, null)
        {
        }
    }
}
