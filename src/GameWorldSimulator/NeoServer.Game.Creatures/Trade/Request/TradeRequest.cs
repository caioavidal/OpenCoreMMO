using System;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creatures.Trade.Request;

public record TradeRequest
{
    public uint FirstPlayer { get; init; }
    public uint SecondPlayer { get; init; }
    public IItem ItemFromFirstPlayer { get; init; }
    public IItem ItemFromSecondPlayer { get; private set; }
    public DateTime RequestedAt { get; init; }
   

    public void UpdateItemFromSecondPlayer(IItem item) => ItemFromSecondPlayer = item;
}