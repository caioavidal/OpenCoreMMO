using System.Collections.Generic;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Item.Structs;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IItemType
    {
        ushort TypeId { get; }

        string Name { get; }

        string Description { get; }

        ISet<ItemFlag> Flags { get; }
        ItemTypeAttribute TypeAttribute { get; }

        ItemGroup Group { get; }

        ushort ClientId { get; }

        ushort WareId { get; }
        LightBlock LightBlock { get; }
        ushort Speed { get; }
        string Article { get; }
        IItemAttributeList Attributes { get; }
        ShootType ShootType { get; }
        AmmoType AmmoType { get; }
        WeaponType WeaponType { get; }
        Slot BodyPosition { get; }
        float Weight { get; }
        ushort TransformTo { get; }
        string Plural { get; }
        IItemAttributeList OnUse { get; }
        DamageType DamageType { get; }
        EffectT EffectT { get; }

        void SetArticle(string article);
        void SetPlural(string plural);

        void SetName(string value);
        void LockChanges();
        void SetSpeed(ushort speed);
        void SetLight(LightBlock lightBlock);
        void SetWareId(ushort wareId);
        bool HasFlag(ItemFlag flag);
        void SetRequirements(IItemRequirement[] requirements);
        void SetOnUse();
    }
}