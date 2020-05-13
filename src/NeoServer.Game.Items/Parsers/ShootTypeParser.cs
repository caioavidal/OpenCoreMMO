using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Parsers
{
    internal class ShootTypeParser
    {
        public static ShootType Parse(string shootType) =>
            shootType switch
            {
                "bolt" => ShootType.Bolt,
                "eartharrow" => ShootType.Bolt,
                "poisonarrow" => ShootType.Bolt,
                "death" => ShootType.Bolt,
                "ice" => ShootType.Bolt,
                "smallearth" => ShootType.Bolt,
                "energy" => ShootType.Bolt,
                "fire" => ShootType.Bolt,
                "smallstone" => ShootType.Bolt,
                "snowball" => ShootType.Bolt,
                "smallice" => ShootType.Bolt,
                "spear" => ShootType.Bolt,
                "throwingstar" => ShootType.Bolt,
                "throwingknife" => ShootType.Bolt,
                "burstarrow" => ShootType.Bolt,
                "powerbolt" => ShootType.Bolt,
                "huntingspear" => ShootType.Bolt,
                "infernalbolt" => ShootType.Bolt,
                "piercingbolt" => ShootType.Bolt,
                "sniperarrow" => ShootType.Bolt,
                "onyxarrow" => ShootType.Bolt,
                "greenstar" => ShootType.Bolt,
                "enchantedspear" => ShootType.Bolt,
                "redstar" => ShootType.Bolt,
                "royalspear" => ShootType.Bolt,
                "flasharrow" => ShootType.Bolt,
                "shiverarrow" => ShootType.Bolt,
                "flammingarrow" => ShootType.Bolt,
                _ => ShootType.None
            };


    }
}
