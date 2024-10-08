using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;

namespace NeoServer.Application.Features.Combat.PlayerDefense;

public static class ShieldDefenseMethod
{
    public static int Defend(IPlayer player, CombatDamage attack)
    {
        var defense = player.Inventory.TotalDefense * player.Skills[SkillType.Shielding].Level *
                      (player.DefenseFactor / 100d) -
                      attack.Damage / 100d * player.ArmorRating * (player.Vocation.Formula?.Defense ?? 1f);

        var resultDamage = (int)(attack.Damage - defense);
        if (resultDamage <= 0) player.IncreaseSkillCounter(SkillType.Shielding, 1);
        return resultDamage;
    }
}

public static class ArmorDefenseMethod
{
    public static int Defend(IPlayer player, int damage)
    {
        switch (player.ArmorRating)
        {
            case > 3:
            {
                var min = player.ArmorRating / 2 * (player.Vocation.Formula?.Armor ?? 1f);
                var max = (player.ArmorRating / 2 * 2 - 1) * (player.Vocation.Formula?.Armor ?? 1f);
                damage -= (ushort)GameRandom.Random.NextInRange(min, max);
                break;
            }
            case > 0:
                --damage;
                break;
        }

        return damage;
    }
}

public static class ImmunityDefenseMethod
{
    public static CombatDamage Defend(IPlayer player, CombatDamage damage)
    {
        if (!player.HasImmunity(damage.Type.ToImmunity())) return damage;
        damage.SetNewDamage(0);
        return damage;
    }
}

public static class EquipmentDefenseMethod
{
    public static CombatDamage Defend(IPlayer player, CombatDamage damage)
    {
        foreach (var equipment in player.Inventory.DressingEquipments)
        {
            if (equipment is not IProtection protection) continue;

            protection.Protect(ref damage);
        }

        return damage;
    }
}

