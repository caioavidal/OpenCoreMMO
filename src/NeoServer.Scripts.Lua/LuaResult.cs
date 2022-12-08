using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NeoServer.Game.Common;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Scripts.Lua;

public class LuaResult
{
    public bool Success { get; }
    private InvalidOperation InvalidOperation { get; }
    public string Error => InvalidOperation.ToString();
    public string ErrorMessage => TextMessageOutgoingParser.Parse(InvalidOperation);
    public LuaResult(bool success, InvalidOperation invalidOperation)
    {
        Success = success;
        InvalidOperation = invalidOperation;
    }
}