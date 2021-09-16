using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface ISkillBonus
    {
        void AddSkillBonus(IPlayer player);
        void RemoveSkillBonus(IPlayer player);
        void ChangeSkillBonuses(Dictionary<SkillType, byte> skillBonuses);
    }
}
