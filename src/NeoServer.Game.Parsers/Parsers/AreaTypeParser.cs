using NeoServer.Game.Effects.Magical;

namespace NeoServer.Game.Effects.Parsers
{
    public class AreaTypeParser
    {
        public static AreaType Parse(string areaType)
        {
            return areaType switch
            {
                "AREA_CIRCLE3X3" => AreaType.AreaCircle3x3
            };
        }
        public static byte[,] Parse(AreaType areaType)
        {
            return areaType switch
            {
                AreaType.AreaCircle3x3 => AreaEffect.Circle3x3
            };
        }
    }
}
