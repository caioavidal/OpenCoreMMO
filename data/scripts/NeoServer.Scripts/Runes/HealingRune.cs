using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Items.Items.UsableItems.Runes;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Scripts.Runes
{
    public class HealingRune : Rune, IConsumable, IUseableOnCreature, IUseableOn
    {
        public HealingRune(IItemType type, Location location, IDictionary<Game.Common.ItemAttribute, System.IConvertible> attributes) : base(type, location, attributes) { }
        public override ushort Duration => 0;
        public event Use OnUsed;
        public EffectT Effect => EffectT.GlitterBlue;
        public void Use(IPlayer usedBy, ICreature creature)
        {
            var minMax = Formula(usedBy, usedBy.Level, usedBy.Skills[SkillType.Magic].Level);
            var healValue = (ushort)GameRandom.Random.Next(minValue: minMax.Min, maxValue: minMax.Max);
            if (creature is ICombatActor actor) actor.Heal(healValue);

            Reduce();

            OnUsed?.Invoke(usedBy, creature, this);
        }
    }
}
