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

        public Monster(IMonsterType type) : base(type)
        {
            Metadata = type;
        }
        public Monster(IMonsterType type, ISpawnPoint spawn) : base(type)
        {
            Metadata = type;
            Spawn = spawn;
        }

        public event Born OnWasBorn;

        public void Reborn()
        {
            ResetHealthPoints();
            SetNewLocation(Spawn.Location);
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public new ushort Speed => Metadata.Speed;

        public override ushort AttackPower => throw new NotImplementedException();

        public override ushort ArmorRating => throw new NotImplementedException();

        public override ushort DefensePower => throw new NotImplementedException();

        public override byte AutoAttackRange => throw new NotImplementedException();


        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set ; }

        public override ushort MinimumAttackPower => 0;

        public override bool UsingDistanceWeapon => throw new NotImplementedException();

        public ISpawnPoint Spawn { get; }
    }
}
