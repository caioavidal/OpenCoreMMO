﻿using System.Diagnostics.CodeAnalysis;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Tasks;
using NeoServer.Application.Features.Item.Decay;
using NeoServer.Game.Common.Contracts.World;
using GameState = NeoServer.Application.Common.Enums.GameState;

namespace NeoServer.Application.Server;

public class GameServer : IGameServer
{
    public GameServer(IMap map,
        IDispatcher dispatcher, IScheduler scheduler, IGameCreatureManager creatureManager,
        IItemDecayTracker itemDecayBag, IPersistenceDispatcher persistenceDispatcher)
    {
        Map = map;
        Dispatcher = dispatcher;
        Scheduler = scheduler;
        CreatureManager = creatureManager;
        ItemDecayTracker = itemDecayBag;
        PersistenceDispatcher = persistenceDispatcher;
        Instance = this;
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public static IGameServer Instance { get; private set; }

    public IItemDecayTracker ItemDecayTracker { get; }

    /// <summary>
    ///     Game's light level
    /// </summary>
    public byte LightLevel => 250;

    /// <summary>
    ///     Indicates Game's light color
    /// </summary>
    public byte LightColor => 215;

    /// <summary>
    ///     Game state
    /// </summary>
    /// <value></value>
    public GameState State { get; private set; }

    /// <summary>
    ///     Map instance
    /// </summary>
    /// <value></value>
    public IMap Map { get; }

    public IGameCreatureManager CreatureManager { get; }
    public IPersistenceDispatcher PersistenceDispatcher { get; }

    /// <summary>
    ///     Dispatcher instance
    /// </summary>
    /// <value></value>
    public IDispatcher Dispatcher { get; }

    /// <summary>
    ///     Scheduler instance
    /// </summary>
    /// <value></value>
    public IScheduler Scheduler { get; }

    /// <summary>
    ///     Sets game state as opened
    /// </summary>
    public void Open()
    {
        State = GameState.Opened;
        OnOpened?.Invoke();
    }

    /// <summary>
    ///     Sets game state as closed
    ///     No one can logIn on game expect GM
    /// </summary>
    public void Close()
    {
        State = GameState.Closed;
    }

    public event OpenServer OnOpened;
}