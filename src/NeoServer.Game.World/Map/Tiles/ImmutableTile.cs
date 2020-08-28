using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Location.Structs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public readonly struct ImmutableTile : IImmutableTile
    {
        public ImmutableTile(Coordinate coordinate, params IItem[] items)
        {
            Location = new Location(coordinate.X, coordinate.Y, coordinate.Z);
            Raw = null;
            Raw = GetRaw(items);
        }

        public byte[] Raw { get; }

        public Location Location { get; }

        public byte[] GetRaw(IItem[] items)
        {


            var ground = new List<byte>();
            var top1 = new List<byte>();
            var downItems = new List<byte>();

            foreach (var item in items)
            {
                if (item is IGround)
                {

                    ground.AddRange(BitConverter.GetBytes(item.ClientId));
                    continue;
                }

                if (item.IsAlwaysOnTop)
                {
                    top1.AddRange(BitConverter.GetBytes(item.ClientId));

                }
                else
                {
                    downItems.AddRange(BitConverter.GetBytes(item.ClientId));

                }
            }

            return ground.Concat(top1).Concat(downItems).ToArray();

        }

        public override int GetHashCode()
        {
            return HashHelper.Start
                .CombineHashCode(Raw);
        }
    }
}
