using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Runes;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Formula => Metadata.Attributes.GetAttribute(ItemAttribute.Formula);

        public Dictionary<string, (double, double)> Variables
        {
            get
            {
                var values =  Metadata.Attributes.GetInnerAttributes(ItemAttribute.Formula).ToDictionary<string, string[]>();
                var dictionary = new Dictionary<string, (double, double)>(values.Count);

                foreach (var value in values)
                {
                    dictionary.Add(value.Key, (double.Parse(value.Value[0], CultureInfo.InvariantCulture), double.Parse(value.Value[1], CultureInfo.InvariantCulture)));
                }
                return dictionary;
            }
        }

        public static new bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type)?.Equals("rune", StringComparison.InvariantCultureIgnoreCase) ?? false;

        public MinMax MinMaxFormula(IPlayer player)
        {
            var variables = Variables;
            var arrayLength = variables.Count + player.FormulaVariables.Length;
            var maxVariables = new (string, double)[arrayLength];
            var minVariables = new (string, double)[arrayLength];

            int i = 0;
            foreach (var variable in variables)
            {
                minVariables[i] = (variable.Key, Math.Min(variable.Value.Item1, variable.Value.Item2));
                maxVariables[i++] = (variable.Key, Math.Max(variable.Value.Item1, variable.Value.Item2));
            }
            foreach (var variable in player.FormulaVariables)
            {
                minVariables[i] = variable;
                maxVariables[i++] = variable;
            }

            var min = (int)StringCalculation.Calculate(Formula, minVariables);
            var max = (int)StringCalculation.Calculate(Formula, maxVariables);

            return new MinMax(min, max);
        }
    }
}
