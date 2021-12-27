using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items.Factories;
using NeoServer.Loaders.Vocations;

namespace NeoServer.Extensions.Spells.Commands
{
    public class ReloadCommand : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotPossible;
            
            if (Params is null || !Params.Any())
            {
                OperationFailService.Display(actor.CreatureId, "Invalid module");
                return false;
            }
            
            Console.Write(Params.Length);
            
            var module = Params[0].ToString();

            if (!Modules.TryGetValue(module, out var action))
            {
                OperationFailService.Display(actor.CreatureId, "Invalid module");
                return false;
            }
            
            action?.Invoke();
            return true;
        }
        
        private static Dictionary<string, Action> Modules => new()
        {
           ["vocations"] = VocationLoader.Instance.Reload 
        };

        private IItem Item(ICombatActor actor, int amount)
        {
            if (ushort.TryParse(Params[0].ToString(), out var typeId))
            {
               return ItemFactory.Instance.Create(typeId, actor.Location,
                    new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, amount } });
            }

            var item = ItemFactory.Instance.Create(Params[0].ToString(), actor.Location,
                new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, amount } });
            
            return item;
        }
    }
}