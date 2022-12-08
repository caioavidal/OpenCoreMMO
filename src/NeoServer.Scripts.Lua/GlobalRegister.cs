using System;
using System.IO;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Services;
using NeoServer.Scripts.Lua.Functions;
using NeoServer.Scripts.Lua.Functions.Libs;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using NLua;
using Serilog;

namespace NeoServer.Scripts.Lua;

public class LuaGlobalRegister
{
    private readonly IItemService _itemService;
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
        IDecayableItemManager decayableItemManager, IItemService itemService)
    {
        this.gameServer = gameServer;
        this.itemFactory = itemFactory;
        this.creatureFactory = creatureFactory;
        this.lua = lua;
        this.serverConfiguration = serverConfiguration;
        this.logger = logger;
        this.coinTransaction = coinTransaction;
        this.decayableItemManager = decayableItemManager;
        _itemService = itemService;
    }

    public void Register()
    {
        logger.Step("Loading lua scripts...", "{Lua} scripts loaded", () =>
        {
            lua.LoadCLRPackage();

            lua["gameServer"] = gameServer;
            lua["sendNotification"] = NotificationSenderService.Send;
            lua["sendOperationFail"] = (IPlayer player, string message) => OperationFailService.Send(player, message);
            lua["scheduler"] = gameServer.Scheduler;
            lua["map"] = gameServer.Map;
            lua["itemFactory"] = itemFactory;
            lua["creatureFactory"] = creatureFactory;
            lua["dofile"] = new Action<string>(DoFile);
            lua["loadfile"] = LoadFile;
            lua["logger"] = logger;
            lua["coinTransaction"] = coinTransaction;
            lua["random"] = GameRandom.Random;
            lua["decayableManager"] = decayableItemManager;
            lua["register"] = RegisterItemAction;
            lua["itemService"] = _itemService;

            lua.AddQuestFunctions();
            lua.AddPlayerFunctions();
            lua.AddItemFunctions();
            lua.AddTileFunctions();
            lua.AddLibs();

            lua["make_array"] = (string typeName, LuaTable x) =>
            {
                if (typeName == "ushort")
                {
                    var values = new ushort[x.Values.Count];
                    var i = 0;
                    foreach (var key in x.Values) values[i++] = Convert.ToUInt16(key);

                    return values;
                }

                throw new Exception("Type not found");
            };

            ExecuteMainFiles();

            return new object[] { "LUA" };
        });
    }

    private void RegisterItemAction(string eventName, LuaFunction func, params object[] keys)
    {
        ItemActionMap.Register(string.Join("-", keys), eventName, func);
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

    private void DoFile(string luaPath)
    {
        lua.DoFile(Path.Combine(serverConfiguration.Data, luaPath));
    }

    private void LoadFile(string luaPath)
    {
        lua.LoadFile(Path.Combine(serverConfiguration.Data, luaPath));
    }
}