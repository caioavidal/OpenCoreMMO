using System;

namespace NeoServer.Game.Common
{
    public class OperationFailService
    {
        public static event Action<uint, string> OperationFailed;

        public static void Display(uint playerId, string message)
        {
            OperationFailed?.Invoke(playerId, message);
        }
    }
}
