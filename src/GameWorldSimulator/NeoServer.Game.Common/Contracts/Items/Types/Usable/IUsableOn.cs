using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public interface IUsableOn : IItem
{
    public EffectT Effect => Metadata.Attributes.GetEffect();

    public int CooldownTime => Metadata.Attributes.HasAttribute(ItemAttribute.CooldownTime)
        ? Metadata.Attributes.GetAttribute<int>(ItemAttribute.CooldownTime)
        : 1000;


    public static Func<IItem, IPlayer, IThing, bool> UseFunction { get; set; }
    public bool UseOn(IPlayer usedBy, IThing onThing)
    {
        return UseFunction?.Invoke(this, usedBy, onThing) ?? true;
    }
}