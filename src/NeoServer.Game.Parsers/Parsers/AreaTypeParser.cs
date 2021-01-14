using NeoServer.Game.DataStore;
using NeoServer.Game.Effects.Magical;

namespace NeoServer.Game.Effects.Parsers
{
    public class AreaTypeParser
    {

        public static byte[,] Parse(string areaType) => AreaTypeStore.Get(areaType);
        
    }
}
