using NeoServer.Game.Common;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Scripts.Lua;

public class LuaResult
{
    public LuaResult(bool success, InvalidOperation invalidOperation)
    {
        Success = success;
        InvalidOperation = invalidOperation;
    }

    public bool Success { get; }
    private InvalidOperation InvalidOperation { get; }
    public string Error => InvalidOperation.ToString();
    public string ErrorMessage => TextMessageOutgoingParser.Parse(InvalidOperation);
}