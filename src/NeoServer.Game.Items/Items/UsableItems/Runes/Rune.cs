using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Runes;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NeoServer.Game.Items.Items.UsableItems.Runes
{
    public abstract class Rune : Cumulative, IRune
    {
        public Rune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        protected Rune(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }

        public CooldownTime Cooldown { get; protected set; }
        public abstract ushort Duration { get; }

        public virtual MinMax Formula(IPlayer player, int level, int magicLevel)
        {
            var variables = Variables;
            variables.TryGetValue("x", out var x);
            variables.TryGetValue("y", out var y);

            var min = (int)((level / 5) + (magicLevel * Math.Min(x.Item1, x.Item2)) + Math.Min(y.Item1, y.Item2));
            var max = (int)((level / 5) + (magicLevel * Math.Max(x.Item1, x.Item2)) + Math.Min(y.Item1, y.Item2));

            return new MinMax(min, max);
        }

        public Dictionary<string, (double, double)> Variables
        {
            get
            {
                Func<string, double> parse = (value) => double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : default;

                var x = Metadata.Attributes.GetAttributeArray("x");
                var y = Metadata.Attributes.GetAttributeArray("y");
                var dictionary = new Dictionary<string, (double, double)>(2)
                {
                    {"x", (parse(x[0]),parse(x[1])) },
                    {"y", (parse(y[0]),parse(y[1])) }
                };
                return dictionary;
            }
        }

     

        public static new bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type)?.Equals("rune", StringComparison.InvariantCultureIgnoreCase) ?? false;
    }
}
