using NeoServer.Game.Contracts.World;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
