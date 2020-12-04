using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Creatures.Vocations
{
    public class VocationStore
    {
        private static ImmutableDictionary<VocationType, IVocation> _vocations;
        private static bool loaded;
        public static void Load(IEnumerable<IVocation> vocations)
        {
            if (loaded) return;

            _vocations = vocations.ToImmutableDictionary(x => x.VocationType, x => x);
            loaded = true;
        }
        public static bool TryGetValue(VocationType type, out IVocation vocation) => _vocations.TryGetValue(type, out vocation);

    }
}
