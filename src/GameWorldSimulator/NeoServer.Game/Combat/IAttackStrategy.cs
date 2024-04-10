using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Combat;

public interface IAttackStrategy
{
    string Name { get; }
    Result Execute(in AttackInput attackInput);
}