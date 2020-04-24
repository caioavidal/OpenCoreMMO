using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Location.Structs.Helpers;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            //Span<byte> ground = stackalloc byte[2];
            //Span<byte> topItems1 = stackalloc byte[27];
            //Span<byte> topItems2 = stackalloc byte[27];
            //Span<byte> downItems = stackalloc byte[27];

            //bool hasGround = false;
            //int countTopItems1 = 0;
            //int countTopItems2 = 0;
            //int countDownItems = 0;

            //foreach (var item in items)
            //{
            //    if (item is IGroundItem)
            //    {
            //        hasGround = true;
            //        BitConverter.GetBytes(item.ClientId).AsSpan().CopyTo(ground);
            //    }

            //    if (item is IAlwaysTopItem)
            //    {
            //        var topOrder = (item as IAlwaysTopItem).TopOrder;

            //        if (topOrder == 1)
            //        {
            //            AddToTopItems(in item, ref topItems1, ref countTopItems1);
            //        }
            //        if (topOrder == 2)
            //        {
            //            AddToTopItems(in item, ref topItems2, ref countTopItems2);
            //        }
            //        else
            //        {
            //            AddToTopItems(in item, ref downItems, ref countDownItems);
            //        }
            //    }
            //}
            //int size = 4 + (countTopItems1 + countTopItems2 + countDownItems) * 2;
            //Span<byte> cachedItems = stackalloc byte[size];

            //if (hasGround)
            //{
            //    ground.CopyTo(cachedItems.Slice(0, 2));
            //}

            //if (countTopItems1 > 0)
            //{
            //    topItems1.Slice(0, countTopItems1).CopyTo(cachedItems.Slice(2, countTopItems1));
            //}
            //if (countTopItems2 > 0)
            //{
            //    topItems2.Slice(0, countTopItems2).CopyTo(cachedItems.Slice(countTopItems1 + 2, countTopItems2));
            //}
            //if (countDownItems > 0)
            //{
            //    downItems.Slice(0, countDownItems).CopyTo(cachedItems.Slice(countTopItems2 + countTopItems1 + 2, countDownItems));
            //}

            //return cachedItems.ToArray();

            var ground = new List<byte>();
            var top1 = new List<byte>();
            var top2 = new List<byte>();
            var downItems = new List<byte>();

            foreach (var item in items)
            {
                if (item is IGroundItem)
                {

                    ground.AddRange(BitConverter.GetBytes(item.ClientId));
                    continue;
                }

                if (item.IsAlwaysOnTop)
                {
                    //var topOrder = (item as IAlwaysTopItem).TopOrder;

                    top1.AddRange(BitConverter.GetBytes(item.ClientId));
                    
                }else if (item.IsBottom)
                {
                    top2.AddRange(BitConverter.GetBytes(item.ClientId));
                }
                else
                {
                    downItems.AddRange(BitConverter.GetBytes(item.ClientId));

                }
            }

            return ground.Concat(top1).Concat(top2).Concat(downItems).ToArray();

        }

        public override int GetHashCode()
        {
            return HashHelper.Start
                .CombineHashCode(Raw);
        }
    }
}
