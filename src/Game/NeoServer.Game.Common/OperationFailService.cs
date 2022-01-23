using System;

namespace NeoServer.Game.Common;

public static class OperationFailService
{
    public static event Action<uint, string> OnOperationFailed;

    public static void Display(uint playerId, string message)
    {
        OnOperationFailed?.Invoke(playerId, message);
    }
}