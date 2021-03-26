using NeoServer.Game.Contracts.World;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;

namespace NeoServer.Loaders.Configurations
{
    public class ConfigurationLoader : IStartupLoader
    {
        private readonly IPathFinder pathFinder;

        public ConfigurationLoader(IPathFinder pathFinder)
        {
            this.pathFinder = pathFinder;
        }

        public void Load()
        {
            ConfigurationStore.PathFinder = pathFinder;
        }
    }
}
