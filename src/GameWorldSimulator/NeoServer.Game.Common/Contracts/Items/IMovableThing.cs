namespace NeoServer.Game.Common.Contracts.Items;

public interface IMovableThing : IThing
{
    void OnMoved(IThing to);
}