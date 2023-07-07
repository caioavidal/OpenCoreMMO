using System;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IGameBehaviorModifierStore : IDataStore<string, Type>
{
}