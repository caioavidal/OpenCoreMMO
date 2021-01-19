using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Combat
{
    public interface ICombatDefense : IProbability
    {
        ushort Interval { get; init; }

        void Defende(ICombatActor actor);
    }
}
