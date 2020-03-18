using NeoServer.Server.Model.Items.Contracts;
using NeoServer.Server.Model.World.Structs;
using NeoServer.Server.World.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public abstract class Thing : IThing
    {
        public event OnThingStateChanged OnThingChanged;

        protected ITile tile;
        protected Location location;

        public const ushort CreatureThingId = 0x63;

        public abstract ushort ThingId { get; }

        public abstract byte Count { get; }

        public abstract string InspectionText { get; }

        public abstract string CloseInspectionText { get; }

        public abstract bool CanBeMoved { get; }

        public Location Location
        {
            get
            {
                return location;
            }

            protected set
            {
                var oldValue = location;
                location = value;
                if (oldValue != location)
                {
                    OnThingChanged?.Invoke(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(Location) });
                }
            }
        }

        public ITile Tile
        {
            get
            {
                return tile;
            }

            set
            {
                if (value != null)
                {
                    Location = value.Location;
                }
                tile = value;
            }
        }

        public void Added()
        {
            // OnThingAdded?.Invoke();
        }

        public void Removed()
        {
            // OnThingRemoved?.Invoke();
        }
    }
}
