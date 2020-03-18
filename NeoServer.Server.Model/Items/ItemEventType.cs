using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
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
