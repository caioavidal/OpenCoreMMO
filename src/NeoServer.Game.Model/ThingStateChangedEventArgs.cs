using System;
using NeoServer.Game.Contracts;

namespace NeoServer.Game.Model
{
    public class ThingStateChangedEventArgs : EventArgs, IThingStateChangedEventArgs
    {
        public string PropertyChanged { get; set; }
    }
}
