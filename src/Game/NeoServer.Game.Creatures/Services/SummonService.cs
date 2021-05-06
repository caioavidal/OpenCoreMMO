using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Monsters;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Services
{
    public class SummonService : ISummonService
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
            if (creatureFactory.CreateSummon(summonName, master) is not Summon summon)
            {
                logger.Error($"Summon with name: {summonName} does not exists");
                return null;
            }

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
    }
}
