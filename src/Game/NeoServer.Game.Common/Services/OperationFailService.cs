using System;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Services;

public static class OperationFailService
{
    public static event Action<uint, string> OnOperationFailed;
    public static event Action<uint, InvalidOperation> OnInvalidOperation;

    public static void Display(uint playerId, string message)
    {
        OnOperationFailed?.Invoke(playerId, message);
    }

    public static void Display(uint playerId, InvalidOperation operation)
    {
        OnInvalidOperation?.Invoke(playerId, operation);
    }

    public static void Display(IPlayer player, string message)
    {
        Display(player.CreatureId, message);
    }

    public static void Display(IPlayer player, InvalidOperation operation)
    {
        Display(player.CreatureId, operation);
    }
}