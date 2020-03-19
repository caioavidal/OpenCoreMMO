using NeoServer.Server.Map;

namespace NeoServer.Server.World
{
    public interface IWorldLoader : IMapLoader
    {
        void Load();
    }
}