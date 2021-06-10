using NeoServer.Game.Common.Contracts.Spells;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface INeedsCooldowns
    {
        void StartSpellCooldown(ISpell spell);
    }
}