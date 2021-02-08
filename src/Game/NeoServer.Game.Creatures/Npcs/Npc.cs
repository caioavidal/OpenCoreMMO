using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
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
            Metadata = type;
        }
        public event Answer OnAnswer;
        public override IOutfit Outfit { get; protected set; }
        public INpcType Metadata { get; }
        public CreateItem CreateNewItem { protected get; init; }

        IDictionary<uint, List<byte>> PlayerDialogTree { get; set; } = new Dictionary<uint, List<byte>>();

        public event Hear OnHear;

        private INpcDialog GetNextAnswer(uint creatureId, string message)
        {
            if (creatureId == 0 || string.IsNullOrWhiteSpace(message)) return null;

            List<byte> positions = null;
            if (!PlayerDialogTree.TryGetValue(creatureId, out positions))
            {
                positions = new List<byte>() { 0 };
            }

            var dialog = GetAnwser(positions, message);

            if (dialog is null) return default;

            if (dialog.End) PlayerDialogTree.Remove(creatureId);
            else PlayerDialogTree.TryAdd(creatureId, positions);

            return dialog;
        }

        private INpcDialog GetAnwser(List<byte> positions, string message)
        {
            INpcDialog[] dialogs = null;
            var i = 0;
            foreach (var position in positions)
            {
                dialogs = i++ == 0 ? Metadata.Dialog : dialogs[position].Then;
            }

            i = 0;

            foreach (var dialog in dialogs)
            {
                if (dialog.OnWords.Any(x => x.Equals(message, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (dialog.Then is not null) positions.Add((byte)i);
                    return dialog;
                }

                i++;
            }
            return null;
        }

        public virtual void Answer(ICreature from, SpeechType speechType, string message)
        {
            if (from is null || string.IsNullOrWhiteSpace(message)) return;

            if (from is not ISociableCreature sociableCreature) return;

            //if it is not the first message to npc and player sent it from any other channel
            if (PlayerDialogTree.ContainsKey(from.CreatureId) && speechType != SpeechType.PrivatePlayerToNpc) return;

            var dialog = GetNextAnswer(from.CreatureId, message);

            if (dialog is null || dialog?.Answers is null) return;

            SendMessageTo(sociableCreature, speechType, dialog);

            OnAnswer?.Invoke(this, from, dialog, message, speechType);
        }

        public virtual void SendMessageTo(ISociableCreature to, SpeechType type, INpcDialog dialog)
        {
            if (dialog is null || dialog.Answers is null || to is null) return;

            foreach (var answer in dialog.Answers)
            {
                if (string.IsNullOrWhiteSpace(answer) || to is not IPlayer) continue;

                Say(answer, SpeechType.PrivateNpcToPlayer, to);
            }
        }

        public void Hear(ICreature from, SpeechType speechType, string message)
        {
            if (from is null || speechType == SpeechType.None || string.IsNullOrWhiteSpace(message)) return;

            OnHear?.Invoke(from, this, speechType, message);

            Answer(from, speechType, message);
        }

        private string ReplaceKeywords(string message, ICreature creature)
        {
            message = message.Replace("|PLAYERNAME|", creature.Name);
            return message;
        }

        public void StopTalkingToCustomer(IPlayer player)
        {
            PlayerDialogTree.Remove(player.CreatureId);
        }
    }
}
