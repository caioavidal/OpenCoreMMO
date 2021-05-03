using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Services
{
    public class SummonService: ISummonService
    {
        private readonly IMonsterFactory monsterFactory;
        private readonly IMap map;
        public SummonService(IMonsterFactory monsterFactory, IMap map)
        {
            this.monsterFactory = monsterFactory;
            this.map = map;
        }

        public IMonster Summon(IMonster master, string summonName)
        {
            var summon = monsterFactory.Create(summonName);

            master.OnKilled += (_, _, _) => OnMasterKilled(map, master, summon);
            summon.OnKilled += (_, _, _) => OnSummonKilled(map, summon);
            return summon;
        }

        private void OnMasterKilled(IMap map, IMonster master, IMonster summon)
        {
            master.OnKilled -= (_, _, _) => OnMasterKilled(map, master, summon);
            map.RemoveCreature(summon);
        }
        private void OnSummonKilled(IMap map, IMonster summon)
        {
            summon.OnKilled -= (_, _, _) => OnSummonKilled(map, summon);
            map.RemoveCreature(summon);
        }
    }
}
