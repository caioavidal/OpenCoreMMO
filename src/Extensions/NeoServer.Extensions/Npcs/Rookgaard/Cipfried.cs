using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creatures.Npcs;

namespace NeoServer.Extensions.Npcs.Rookgaard;

public class Cipfried : Npc
{
    public Cipfried(INpcType type, IMapTool mapTool, ISpawnPoint spawnPoint, IOutfit outfit = null,
        uint healthPoints = 0) : base(type, mapTool, spawnPoint, outfit, healthPoints)
    {
        OnDialogAction += OnDialogActionHandler;
    }

    private void OnDialogActionHandler(INpc from, ICreature to, IDialog dialog, string action,
        Dictionary<string, string> lastkeywords)
    {
        if (action is null) return;

        Dictionary<string, dynamic> customData = from.Metadata.CustomAttributes["custom-data"];
        HealAnswer(from, to, action, customData);
        HiAnswer(from, to, action, customData);
    }

    private static void HiAnswer(INpc from, ICreature to, string action, Dictionary<string, dynamic> customData)
    {
        if (action != "hi") return;
        if (to.HealthPoints > 65) return;

        if (to is IPlayer player) player.Heal((ushort)Math.Max(0, 65 - player.HealthPoints), from);
    }

    private static void HealAnswer(INpc from, ICreature to, string action, Dictionary<string, dynamic> customData)
    {
        if (action != "heal") return;
        if (to.HealthPoints >= 185)
        {
            from.Say(customData["cant_heal"], SpeechType.PrivateNpcToPlayer, to);
            return;
        }

        if (to is not IPlayer player) return;

        player.Heal((ushort)Math.Max(0, 185 - player.HealthPoints), from);
        from.Say(customData["can_heal"], SpeechType.PrivateNpcToPlayer, to);
    }
}