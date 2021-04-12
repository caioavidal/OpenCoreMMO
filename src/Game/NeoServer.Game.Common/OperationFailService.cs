using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
