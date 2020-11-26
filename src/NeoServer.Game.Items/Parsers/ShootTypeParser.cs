using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Parsers
{
    internal class ShootTypeParser
    {
        public static ShootType Parse(string shootType) =>
            shootType switch
            {
                "bolt" => ShootType.Bolt,
                "eartharrow" => ShootType.EarthArrow,
                "poisonarrow" => ShootType.PoisonArrow,
                "death" => ShootType.Death,
                "ice" => ShootType.Ice,
                "smallearth" => ShootType.SmallEarth,
                "energy" => ShootType.Energy,
                "fire" => ShootType.Fire,
                "smallstone" => ShootType.SmallStone,
                "snowball" => ShootType.SnowBall,
                "smallice" => ShootType.SmallIce,
                "spear" => ShootType.Spear,
                "throwingstar" => ShootType.ThrowingStar,
                "throwingknife" => ShootType.ThrowingKnife,
                "burstarrow" => ShootType.BurstArrow,
                "powerbolt" => ShootType.PowerBolt,
                "huntingspear" => ShootType.HuntingSpear,
                "infernalbolt" => ShootType.InfernalBolt,
                "piercingbolt" => ShootType.PiercingBolt,
                "sniperarrow" => ShootType.SniperArrow,
                "onyxarrow" => ShootType.OnyxArrow,
                "greenstar" => ShootType.GreenStar,
                "enchantedspear" => ShootType.EnchantedSpear,
                "redstar" => ShootType.RedStar,
                "royalspear" => ShootType.RoyalSpear,
                "flasharrow" => ShootType.FlashArrow,
                "shiverarrow" => ShootType.ShiverArrow,
                "flammingarrow" => ShootType.FlammingArrow,
                _ => ShootType.None
            };

    }
}
