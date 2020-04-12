using NeoServer.Game.Contracts;
using System;

namespace NeoServer.Game.Model
{
    public class ThingStateChangedEventArgs : EventArgs, IThingStateChangedEventArgs
    {
        public string PropertyChanged { get; set; }
    }
}
