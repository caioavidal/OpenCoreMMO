using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Game.Creatures.Npcs.Dialogs;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Npcs
{
    public delegate string KeywordReplacement(string message, INpc npc, ISociableCreature to);
    public class Npc : WalkableCreature, INpc
    {
        public Npc(INpcType type, IPathAccess pathAccess, ISpawnPoint spawnPoint, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
            Metadata = type;
            npcDialog = new NpcDialog(this);
            SpawnPoint = spawnPoint;

            Cooldowns.Start(CooldownType.Advertise, 10_000);
            Cooldowns.Start(CooldownType.WalkAround, 5_000);
        }

        #region Events
        public event DialogAction OnDialogAction;
        public event Answer OnAnswer;
        public event Hear OnHear;
        #endregion

        public ISpawnPoint SpawnPoint { get; }
        public override IOutfit Outfit { get; protected set; }
        public INpcType Metadata { get; }
        public CreateItem CreateNewItem { protected get; init; }

        public override ITileEnterRule TileEnterRule => NpcEnterTileRule.Rule;

        public KeywordReplacement ReplaceKeywords { get; set; }

        public override bool CanSeeInvisible => false;

        public override bool CanBeSeen => true;

        private NpcDialog npcDialog;

        public Dictionary<string, string> GetPlayerStoredValues(ISociableCreature sociableCreature) => npcDialog.GetDialogStoredValues(sociableCreature);

        private string BindAnswerVariables(ISociableCreature creature, IDialog dialog, string answer)
        {
            var storedValues = npcDialog.GetDialogStoredValues(creature);
            if (string.IsNullOrWhiteSpace(dialog.StoreAt)) return answer;

            if (!storedValues.TryGetValue(dialog.StoreAt, out var value)) return answer;
            return answer.Replace($"{{{{{dialog.StoreAt}}}}}", value);
        }

        public void Advertise()
        {
            if (!Cooldowns.Cooldowns[CooldownType.Advertise].Expired) return;
            Say(GameRandom.Random.Next(Metadata.Marketings), SpeechType.Say);
            Cooldowns.Start(CooldownType.Advertise, 10_000);
        }

        public void BackInDialog(ISociableCreature creature, byte count) => npcDialog.Back(creature.CreatureId, count);

        public virtual void Answer(ICreature from, SpeechType speechType, string message)
        {
            if (from is null || string.IsNullOrWhiteSpace(message)) return;

            if (from is not ISociableCreature sociableCreature) return;

            //if it is not the first message to npc and player sent it from any other channel
            if (npcDialog.IsTalkingWith(from) && speechType != SpeechType.PrivatePlayerToNpc) return;

            var dialog = npcDialog.GetNextAnswer(from.CreatureId, message);

            if (dialog is null) return;

            npcDialog.StoreWords(sociableCreature, dialog.StoreAt, message);

            if (dialog.Action is not null) OnDialogAction?.Invoke(this, from, dialog, dialog.Action, GetPlayerStoredValues(sociableCreature));

            if (dialog?.Answers is not null)
            {
                SendMessageTo(sociableCreature, speechType, dialog);

                OnAnswer?.Invoke(this, from, dialog, message, speechType);
            }
        }

        public virtual void SendMessageTo(ISociableCreature to, SpeechType type, IDialog dialog)
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

        public override bool WalkRandomStep()
        {
            if (!Cooldowns.Cooldowns[CooldownType.WalkAround].Expired) return false;
            var result = base.WalkRandomStep();
            Cooldowns.Start(CooldownType.WalkAround, 5_000);
            return result;
        }

        public void Hear(ICreature from, SpeechType speechType, string message)
        {
            if (from is null || speechType == SpeechType.None || string.IsNullOrWhiteSpace(message)) return;

            OnHear?.Invoke(from, this, speechType, message);

            Answer(from, speechType, message);
        }

        public void StopTalkingToCustomer(IPlayer player) => npcDialog.StopTalkingTo(player);
    }
}
