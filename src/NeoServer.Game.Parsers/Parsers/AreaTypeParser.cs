using NeoServer.Game.Effects.Magical;

namespace NeoServer.Game.Effects.Parsers
{
    public class AreaTypeParser
    {
      
        public static byte[,] Parse(string areaType)
        {
            return areaType switch
            {
                "AREA_CIRCLE3x3" => AreaEffect.Circle3x3,
                "AREA_SQUARE1x1" => AreaEffect.Square1x1,
            };
        }
    }
}
