using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures;
using NeoServer.Game.World.Map;

namespace NeoServer.Scripts.Spells.Commands
{
    public class MonsterCreator : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotPossible;
            if (Params?.Length == 0) return false;
            
            var monster = CreatureFactory.Instance.CreateMonster(Params[0].ToString());
            if (monster is null) return false;

            var map = Map.Instance;

            foreach (var neighbour in actor.Location.Neighbours)
            {
                
                if (map[neighbour] is IDynamicTile toTile && !toTile.HasCreature)
                {
                    monster.Born(neighbour);
                    return true;
                }
            }
            error = InvalidOperation.NotEnoughRoom;
            return false;
        }
    }
}
