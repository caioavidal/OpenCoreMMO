using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Combat
{
    public interface ICombatDefense : IProbability
    {
        ushort Interval { get; init; }

        void Defende(ICombatActor actor);
    }
}