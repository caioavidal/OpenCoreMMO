using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;

namespace NeoServer.Loaders.Configurations
{
    public class ConfigurationLoader : IStartupLoader
    {
        private readonly IPathFinder pathFinder;
        private readonly IWalkToMechanism walkToMechanism;

        public ConfigurationLoader(IPathFinder pathFinder, IWalkToMechanism walkToMechanism)
        {
            this.pathFinder = pathFinder;
            this.walkToMechanism = walkToMechanism;
        }

        public void Load()
        {
            GameToolStore.PathFinder = pathFinder;
            GameToolStore.WalkToMechanism = walkToMechanism;
        }
    }
}
