using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Services;

public static class OperationFailService
{
    public static event Action<uint, string> OnOperationFailed;
    public static event Action<uint, InvalidOperation> OnInvalidOperation;

    public static void Send(uint playerId, string message)
    {
        OnOperationFailed?.Invoke(playerId, message);
    }

    public static void Send(uint playerId, InvalidOperation error)
    {
        OnInvalidOperation?.Invoke(playerId, error);
    }

    public static void Send(IPlayer player, string message)
    {
        Send(player.CreatureId, message);
    }

    public static void Send(IPlayer player, InvalidOperation error)
    {
        if (error is InvalidOperation.None) return;
        
        Send(player.CreatureId, error);
    }
}