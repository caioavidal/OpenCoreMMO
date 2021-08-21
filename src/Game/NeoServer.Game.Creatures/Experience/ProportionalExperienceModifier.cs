using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Monsters;
using System.Linq;

namespace NeoServer.Game.Creatures.Experience
{
    /// <summary>
    /// Modifies the base experience of the monster that the player receives based on the portion of the damage they dealt.
    /// </summary>
    public class ProportionalExperienceModifier : IBaseExperienceModifier
    {
        public string Name => "Proportional Damage";

        public uint GetModifiedBaseExperience(IPlayer player, IMonster monster, uint baseExperience)
        {
            var totalDamage = monster.Damages.Sum(x => x.Value);
            var playerDamage = monster.Damages.Where(x =>
            {
                if (x.Key == player) { return true; }
                if ((x.Key as Summon)?.Master == player) { return true; }
                return false;
            }).Sum(x => x.Value);
            var percentDamageFactor = (double)playerDamage / (double)totalDamage;
            return (uint)(baseExperience * percentDamageFactor);
        }

        public bool IsEnabled(IPlayer player, IMonster monster)
        {
            return true;
        }
    }
}