using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;
using NeoServer.Game.Items.Items;

namespace NeoServer.Game.Items.Factories
{
    public class DefenseEquipmentFactory : IFactory
    {
        private readonly DecayableFactory _decayableFactory;
        private readonly ProtectionFactory _protectionFactory;
        private readonly SkillBonusFactory _skillBonusFactory;
        private readonly TransformableFactory _transformableFactory;

        public DefenseEquipmentFactory(DecayableFactory decayableFactory, SkillBonusFactory skillBonusFactory,
            ProtectionFactory protectionFactory,
            TransformableFactory transformableFactory)
        {
            _decayableFactory = decayableFactory;
            _skillBonusFactory = skillBonusFactory;
            _protectionFactory = protectionFactory;
            _transformableFactory = transformableFactory;
        }

        public event CreateItem OnItemCreated;

        public BodyDefenseEquipment Create(IItemType itemType, Location location)
        {
            var decayable = _decayableFactory.Create(itemType);
            var skillBonuses = _skillBonusFactory.Create(itemType);
            var protection = _protectionFactory.Create(itemType);
            var transformable = _transformableFactory.Create(itemType);

            if (!BodyDefenseEquipment.IsApplicable(itemType)) return null;

            return new BodyDefenseEquipment(itemType, location)
            {
                Decayable = decayable,
                SkillBonus = skillBonuses,
                Protection = protection,
                Transformable = transformable
            };
        }
    }
}