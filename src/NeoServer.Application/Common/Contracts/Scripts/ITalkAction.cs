﻿using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creature.Player;

namespace NeoServer.Application.Common.Contracts.Scripts
{
    public interface ITalkAction
    {
        bool ExecuteSay(IPlayer player, string words, string param, SpeechType type);
    }
}
