using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Items
{
    public interface IMagicField
    {
        Location Location { get; }
        IItemType Metadata { get; }

        void CauseDamage(ICreature toCreature);
    }
}