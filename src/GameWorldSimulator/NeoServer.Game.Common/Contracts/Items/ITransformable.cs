using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items;

public delegate void Transform(IPlayer by, IItem from, ushort to);

public interface ITransformable
{
    void Transform(IPlayer by);
    event Transform OnTransform;
}