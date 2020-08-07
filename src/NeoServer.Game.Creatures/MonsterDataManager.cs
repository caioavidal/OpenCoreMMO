using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NeoServer.Game.Creature
{
    public class MonsterDataManager : IMonsterDataManager
    {
        private IImmutableDictionary<string, IMonsterType> _monsters;
        private bool _loaded = false;

        public bool TryGetMonster(string name, out IMonsterType monster) => _monsters.TryGetValue(name, out monster);


        public void Load(IEnumerable<IMonsterType> monsters)
        {
            if(_loaded == true)
            {
                throw new InvalidOperationException("Monsters already loaded");
            }

            _monsters = monsters.ToImmutableDictionary(x => x.Name);
            _loaded = true;
        }
    }
}