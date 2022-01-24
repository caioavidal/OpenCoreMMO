using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Npcs.Dialogs;

public class NpcDialog
{
    private readonly INpc npc;

    private readonly IDictionary<uint, Dictionary<string, string>> playerDialogStorage =
        new Dictionary<uint, Dictionary<string, string>>();

    public NpcDialog(INpc npc)
    {
        this.npc = npc;
    }

    private IDictionary<uint, List<byte>> playerDialogTree { get; } = new Dictionary<uint, List<byte>>();

    public bool StopTalkingTo(ICreature creature)
    {
        return playerDialogTree.Remove(creature.CreatureId);
    }

    public bool IsTalkingWith(ICreature creature)
    {
        return playerDialogTree.ContainsKey(creature.CreatureId);
    }

    public Dictionary<string, string> GetDialogStoredValues(ISociableCreature sociableCreature)
    {
        return playerDialogStorage.TryGetValue(sociableCreature.CreatureId, out var map) ? map : null;
    }

    public void StoreWords(ISociableCreature creature, string key, string value)
    {
        if (creature is null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) return;

        if (playerDialogStorage.TryGetValue(creature.CreatureId, out var map))
        {
            if (map.ContainsKey(key)) map[key] = value;
            else map.TryAdd(key, value);

            return;
        }

        playerDialogStorage.TryAdd(creature.CreatureId, new Dictionary<string, string> { { key, value } });
    }

    public void Back(uint creatureId, byte count)
    {
        if (!playerDialogTree.TryGetValue(creatureId, out var dialogHistory)) return;

        for (var i = 0; i < count; i++) dialogHistory.RemoveAt(dialogHistory.Count - 1);
    }

    public IDialog GetNextAnswer(uint creatureId, string message)
    {
        if (creatureId == 0 || string.IsNullOrWhiteSpace(message)) return null;

        if (!playerDialogTree.TryGetValue(creatureId, out var positions)) positions = new List<byte> { 0 };

        var dialog = GetAnswer(positions, creatureId, message);

        if (dialog is null) return default;

        if (dialog.End) EraseDialog(creatureId);
        else playerDialogTree.TryAdd(creatureId, positions);

        return dialog;
    }

    private IDialog GetAnswer(List<byte> positions, uint creatureId, string message)
    {
        IDialog[] dialogs = null;
        var i = 0;
        foreach (var position in positions) dialogs = i++ == 0 ? npc.Metadata.Dialogs : dialogs[position].Then;

        i = 0;

        foreach (var dialog in dialogs)
        {
            if (dialog.OnWords.Any(x => x.Equals(message, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (dialog.Then is not null) positions.Add((byte)i);
                if (dialog.Back > 0) Back(creatureId, dialog.Back);
                return dialog;
            }

            i++;
        }

        return null;
    }

    public void EraseDialog(uint creatureId)
    {
        playerDialogTree.Remove(creatureId);
        playerDialogStorage.Remove(creatureId);
    }
}