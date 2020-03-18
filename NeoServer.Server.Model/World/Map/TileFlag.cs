using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.World.Map
{
    public enum TileFlag : byte
    {
        None = 0,
        Refresh = 1 << 0,
        ProtectionZone = 1 << 1,
        NoLogout = 1 << 2
    }
}
