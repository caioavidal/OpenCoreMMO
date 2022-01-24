using System;
using System.IO;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Helpers;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Serilog;

namespace NeoServer.Scripts.Lua;

public class LuaGlobalRegister
{
    private readonly ICoinTransaction coinTransaction;
    private readonly ICreatureFactory creatureFactory;
    private readonly IDecayableItemManager decayableItemManager;
    private readonly IGameServer gameServer;
    private readonly IItemFactory itemFactory;
    private readonly ILogger logger;
    private readonly NLua.Lua lua;
    private readonly ServerConfiguration serverConfiguration;

    public LuaGlobalRegister(IGameServer gameServer, IItemFactory itemFactory, ICreatureFactory creatureFactory,
        NLua.Lua lua, ServerConfiguration serverConfiguration, ILogger logger, ICoinTransaction coinTransaction,
        IDecayableItemManager decayableItemManager)
    {
        this.gameServer = gameServer;
        this.itemFactory = itemFactory;
        this.creatureFactory = creatureFactory;
        this.lua = lua;
        this.serverConfiguration = serverConfiguration;
        this.logger = logger;
        this.coinTransaction = coinTransaction;
        this.decayableItemManager = decayableItemManager;
    }

    public void Register()
    {
        logger.Step("Loading lua scripts...", "{Lua} scripts loaded", () =>
        {
            lua.LoadCLRPackage();

            lua["gameServer"] = gameServer;
            lua["scheduler"] = gameServer.Scheduler;
            lua["map"] = gameServer.Map;
            lua["itemFactory"] = itemFactory;
            lua["creatureFactory"] = creatureFactory;
            lua["load"] = new Action<string>(DoFile);
            lua["logger"] = logger;
            lua["coinTransaction"] = coinTransaction;
            lua["random"] = GameRandom.Random;
            lua["decayableManager"] = decayableItemManager;

            ExecuteMainFiles();

            return new object[] { "LUA" };
        });
    }

    private void ExecuteMainFiles()
    {
        var dataPath = serverConfiguration.Data;

        foreach (var file in Directory.GetFiles(dataPath, "main.lua", new EnumerationOptions
                 {
                     AttributesToSkip = FileAttributes.Temporary,
                     IgnoreInaccessible = true,
                     RecurseSubdirectories = true
                 }))
            lua.DoFile(file);
    }

    public void DoFile(string luaPath)
    {
        lua.DoFile(Path.Combine(serverConfiguration.Data, luaPath));
    }
}