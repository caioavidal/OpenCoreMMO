using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public class Accessory: IProtectionItem, IDecayable, ISkillBonus, IDefenseEquipmentItem, IChargeable
    {
        public Location Location { get; set; }
        public IItemType Metadata { get; }
        public void DressedIn(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public void UndressFrom(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public int DecaysTo { get; }
        public int Duration { get; }
        public bool Expired { get; }
        public bool StartedToDecay { get; }
        public long StartedToDecayTime { get; }
        public bool ShouldDisappear { get; }
        public bool Decay()
        {
            throw new NotImplementedException();
        }

        public Dictionary<SkillType, byte> SkillBonus { get; }
        public void OnMoved()
        {
            throw new NotImplementedException();
        }

        public ImmutableDictionary<DamageType, byte> DamageProtection { get; }
        public byte Charges { get; }
        public void DecreaseCharges()
        {
            throw new NotImplementedException();
        }
    }
}
