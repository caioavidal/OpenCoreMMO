using NeoServer.Game.Contracts.Spells;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INeedsCooldowns
    {
        void StartSpellCooldown(ISpell spell);
    }
}
