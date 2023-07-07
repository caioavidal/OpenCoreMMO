using System;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Data.InMemory.DataStores;

public class GameBehaviorModifierStore : DataStore<GameBehaviorModifierStore, string, Type>,
    IGameBehaviorModifierStore
{
}