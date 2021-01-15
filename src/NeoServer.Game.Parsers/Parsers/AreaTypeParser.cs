using NeoServer.Game.DataStore;

namespace NeoServer.Game.Effects.Parsers
{
    public class AreaTypeParser
    {

        public static byte[,] Parse(string areaType) => AreaTypeStore.Get(areaType);
        
    }
}
