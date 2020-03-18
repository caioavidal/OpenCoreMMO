using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public class ThingStateChangedEventArgs : EventArgs
    {
        public string PropertyChanged { get; set; }
    }
}
