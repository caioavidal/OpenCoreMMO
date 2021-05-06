using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World.Tiles;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Services
{
    public class SummonService: ISummonService
    {
        private readonly ICreatureFactory creatureFactory;
        private readonly IMap map;
        private readonly Logger logger;
        public SummonService(ICreatureFactory creatureFactory, IMap map, Logger logger)
        {
            this.creatureFactory = creatureFactory;
            this.map = map;
            this.logger = logger;
        }

        public IMonster Summon(IMonster master, string summonName)
        {
            var summon = creatureFactory.CreateMonster(summonName);
            if (summon is null)
            {
                logger.Error($"Summon with name: {summonName} does not exists");
                return null;
            }

            master.OnKilled += (_, _, _) => OnMasterKilled(map, master, summon);
            summon.OnKilled += (_, _, _) => OnSummonKilled(map, summon);

            foreach (var neighbour in master.Location.Neighbours)
            {

                if (map[neighbour] is IDynamicTile toTile && !toTile.HasCreature)
                {
                    summon.Born(toTile.Location);
                    return summon;
                }
            }
            return null;
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
