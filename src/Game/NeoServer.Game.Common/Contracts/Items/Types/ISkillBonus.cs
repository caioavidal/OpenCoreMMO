using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface ISkillBonus
{
    void AddSkillBonus(IPlayer player);
    void RemoveSkillBonus(IPlayer player);
}