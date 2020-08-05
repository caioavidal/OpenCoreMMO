using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public class Monster : Creature, IMonster
    {

        public Monster(uint id, IMonsterType type) : base(id, type)
        {
            Metadata = type;
        }

        public event Born OnWasBorn;

        public void Born()
        {
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public new ushort Speed => Metadata.Speed;

       

        ISpawn Spawn { get; set; }


        public override ushort AttackPower => throw new NotImplementedException();

        public override ushort ArmorRating => throw new NotImplementedException();

        public override ushort DefensePower => throw new NotImplementedException();

        public override byte AutoAttackRange => throw new NotImplementedException();

        public override IInventory Inventory { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set ; }
    }
}
