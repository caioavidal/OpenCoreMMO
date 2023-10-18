namespace NeoServer.Game.Common.Item;

public enum ItemEventType : byte
{
    Use,
    MultiUse,
    Movement,
    Collision,
    Separation
}