using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums
{
    public enum ItemEventType : byte
    {
        Use,
        MultiUse,
        Movement,
        Collision,
        Separation
    }

}
