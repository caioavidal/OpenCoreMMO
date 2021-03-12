using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.DataStore
{
    public class ConfigurationStore : DataStore<ConfigurationStore, string, object>
    {
        public static IPathFinder PathFinder
        {
            get => Data.Get(nameof(PathFinder)) is not IPathFinder pathFinder ? null : pathFinder;
            set => Data.Add(nameof(PathFinder), value);
        }
    }
}
