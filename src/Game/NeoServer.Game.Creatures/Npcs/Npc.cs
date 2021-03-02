using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Npcs
{

    public delegate string KeywordReplacement(string message, INpc npc, ISociableCreature to);
    public class Npc : WalkableCreature, INpc
    {
        public Npc(INpcType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
            Metadata = type;
        }
        public event DialogAction OnDialogAction;
        public event Answer OnAnswer;
        public override IOutfit Outfit { get; protected set; }
        public INpcType Metadata { get; }
        public CreateItem CreateNewItem { protected get; init; }

        public KeywordReplacement ReplaceKeywords { get; set; }

        IDictionary<uint, List<byte>> PlayerDialogTree { get; set; } = new Dictionary<uint, List<byte>>();

        private IDictionary<uint, Dictionary<string, string>> playerStoredValues = new Dictionary<uint, Dictionary<string, string>>();

        public event Hear OnHear;

        public Dictionary<string, string> GetPlayerStoredValues(ISociableCreature sociableCreature) => playerStoredValues.TryGetValue(sociableCreature.CreatureId, out var keywords) ? keywords : null;

        private void StoreWords(ISociableCreature creature, string storeVariableName, string value)
        {
            if (creature is null || string.IsNullOrWhiteSpace(storeVariableName) || string.IsNullOrWhiteSpace(value)) return;

            if (playerStoredValues.TryGetValue(creature.CreatureId, out var keywords))
            {
                keywords.Add(storeVariableName, value);
            }
            playerStoredValues.TryAdd(creature.CreatureId, new Dictionary<string, string>() { { storeVariableName, value } });
        }

        private string BindAnswerVariables(ISociableCreature creature, INpcDialog dialog, string answer)
        {
            var storedValues = GetPlayerStoredValues(creature);
            if (string.IsNullOrWhiteSpace(dialog.StoreAt)) return answer;

            if (!storedValues.TryGetValue(dialog.StoreAt, out var value)) return answer;
            return answer.Replace($"{{{{{dialog.StoreAt}}}}}", value);
        }

        private INpcDialog GetNextAnswer(uint creatureId, string message)
        {
            if (creatureId == 0 || string.IsNullOrWhiteSpace(message)) return null;

            if (!PlayerDialogTree.TryGetValue(creatureId, out List<byte> positions))
            {
                positions = new List<byte>() { 0 };
            }

            var dialog = GetAnswer(positions, message);

            if (dialog is null) return default;

            if (dialog.End) PlayerDialogTree.Remove(creatureId);
            else PlayerDialogTree.TryAdd(creatureId, positions);

            return dialog;
        }

        private INpcDialog GetAnswer(List<byte> positions, string message)
        {
            INpcDialog[] dialogs = null;
            var i = 0;
            foreach (var position in positions)
            {
                dialogs = i++ == 0 ? Metadata.Dialogs : dialogs[position].Then;
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

            if (dialog is null) return;

            StoreWords(sociableCreature, dialog.StoreAt, message);

            if (dialog.Action is not null) OnDialogAction?.Invoke(this, from, dialog, dialog.Action, GetPlayerStoredValues(sociableCreature));

            if (dialog?.Answers is not null)
            {
                SendMessageTo(sociableCreature, speechType, dialog);

                OnAnswer?.Invoke(this, from, dialog, message, speechType);
            }
        }

        public virtual void SendMessageTo(ISociableCreature to, SpeechType type, INpcDialog dialog)
        {
            if (dialog is null || dialog.Answers is null || to is null) return;

            foreach (var answer in dialog.Answers)
            {
                var replacedAnswer = ReplaceKeywords?.Invoke(answer, this, to);
                var bindedAnswer = BindAnswerVariables(to, dialog, replacedAnswer);

                if (string.IsNullOrWhiteSpace(bindedAnswer) || to is not IPlayer) continue;

                Say(bindedAnswer, SpeechType.PrivateNpcToPlayer, to);
            }
        }

        public void Hear(ICreature from, SpeechType speechType, string message)
        {
            if (from is null || speechType == SpeechType.None || string.IsNullOrWhiteSpace(message)) return;

            OnHear?.Invoke(from, this, speechType, message);

            Answer(from, speechType, message);
        }

     
        public void StopTalkingToCustomer(IPlayer player)
        {
            PlayerDialogTree.Remove(player.CreatureId);
        }
    }
}
