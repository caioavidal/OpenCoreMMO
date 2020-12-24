using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Common;

namespace NeoServer.Game.World.Map
{
    public class Sector : Region
    {
        public Sector South { get; set; }
        public Sector East { get; set; }
        public Floor[] Floors { get; set; } = new Floor[16];
        public HashSet<ICreature> Creatures { get; private set; } = new HashSet<ICreature>();
        public HashSet<ICreature> Players { get; private set; } = new HashSet<ICreature>();
        public List<ICreature> SpectatorsCache { get; private set; } = new List<ICreature>(32);

        public Floor GetFloor(byte z) => Floors[z];
        public Floor AddFloor(byte z)
        {
            if (Floors[z].Tiles == null)
            {
                Floors[z].Tiles = new BaseTile[8, 8];
            }

            return Floors[z];
        }

        public void AddCreature(ICreature creature)
        {
            Creatures.Add(creature);
            if (creature is IPlayer player)
            {
                Players.Add(player);
            }
            SpectatorsCache.Clear();
        }
        public void RemoveCreature(ICreature creature)
        {
            Creatures.Remove(creature);

            if (creature is IPlayer player)
                Players.Remove(player);

            SpectatorsCache.Clear();
        }
    }

    public readonly ref struct SpectatorSearch
    {
        public MinMax RangeX { get; }
        public MinMax RangeY { get; }
        public MinMax RangeZ { get; }
        public MinMax Y { get; }
        public MinMax X { get; }
        public bool OnlyPlayers { get; }
        public Location CenterPosition { get; }

        public SpectatorSearch(ref Location center, int minRangeX, int maxRangeX, int minRangeY, int maxRangeY, int minRangeZ, int maxRangeZ, bool onlyPlayers)
        {
            int minY = center.Y + minRangeY;
            int minX = center.X + minRangeX;
            int maxY = center.Y + maxRangeY;
            int maxX = center.X + maxRangeX;

            int minoffset = center.Z - maxRangeZ;
            ushort x1 = (ushort)Math.Min(0xFFFF, Math.Max(0, (minX + minoffset)));
            ushort y1 = (ushort)Math.Min(0xFFFF, Math.Max(0, (minY + minoffset)));

            int maxoffset = center.Z - minRangeZ;
            ushort x2 = (ushort)Math.Min(0xFFFF, Math.Max(0, (maxX + maxoffset)));
            ushort y2 = (ushort)Math.Min(0xFFFF, Math.Max(0, (maxY + maxoffset)));

            RangeX = new MinMax(x1 - (x1 % 8), x2 - (x2 % 8));
            RangeY = new MinMax(y1 - (y1 % 8), y2 - (y2 % 8));
            RangeZ = new MinMax(minRangeZ, maxRangeZ);
            Y = new MinMax(minY, maxY);
            X = new MinMax(minX, maxX);
            CenterPosition = center;
            OnlyPlayers = onlyPlayers;
        }
    }

}
