using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Npcs
{
    public class Npc : WalkableCreature, INpc
    {
        public Npc(INpcType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
            Type = type;
        }
        public override IOutfit Outfit { get; protected set; }
        public INpcType Type { get; }

        IDictionary<uint, List<string>> PlayerDialogTree { get; set; } = new Dictionary<uint, List<string>>();

        public void ReceiveMessage(uint playerId, string message)
        {
            //INpcDialog dialog = GetDialog(playerId);

            //var dialog = Type.Dialog.FirstOrDefault(x => x.OnWords.Contains(message, StringComparer.InvariantCultureIgnoreCase));
            //PlayerDialogTree.Add()
            //Answer(dialog.Answer);
        }

        private INpcDialog GetDialog(uint playerId, INpcDialog dialog = null)
        {
            foreach (var treePoint in PlayerDialogTree[playerId])
            {
                if (dialog is null)
                {
                    dialog = Type.Dialog[treePoint];
                }
            }

            return dialog;
        }

        public void Answer()
        {
            
        }
    }
}
