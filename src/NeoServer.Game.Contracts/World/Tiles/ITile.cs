using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.World
{
    public interface ITile
    {
        Location Location { get; }

        /// <summary>
        /// check whether tile is 1 sqm distant to destination tile
        /// </summary>
        /// <returns></returns>
        public bool IsNextTo(ITile dest) => Location.IsNextTo(dest.Location);
    }
}
