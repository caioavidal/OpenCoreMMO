// using System.Collections.Generic;
// using System.Collections.Immutable;
// using NeoServer.Game.Common.Contracts.Creatures;
//
// namespace NeoServer.Game.Creatures.Vocations
// {
//     public class VocationStore
//     {
//         private static ImmutableDictionary<byte, IVocation> _vocations;
//         private static bool loaded;
//
//         public static void Load(IEnumerable<IVocation> vocations)
//         {
//             if (loaded) return;
//
//             _vocations = vocations.ToImmutableDictionary(x => x.VocationType, x => x);
//             loaded = true;
//         }
//
//         public static bool TryGetValue(byte type, out IVocation vocation)
//         {
//             return _vocations.TryGetValue(type, out vocation);
//         }
//     }
// }

