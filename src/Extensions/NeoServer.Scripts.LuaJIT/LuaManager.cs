﻿using LuaNET;
using NeoServer.Application.Common.Contracts.Scripts;
using Serilog;
using System.Reflection.Metadata.Ecma335;

namespace NeoServer.Scripts.LuaJIT;

public class LuaManager : ILuaManager
{
    #region Members

    #endregion

    #region Injection

    /// <summary>
    /// A reference to the logger instance in use.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// A reference to the lua enviroment instance in use.
    /// </summary>
    private readonly ILuaEnvironment _luaEnviroment;

    /// <summary>
    /// A reference to the config manager instance in use.
    /// </summary>
    private readonly IConfigManager _configManager;

    /// <summary>
    /// A reference to the scripts instance in use.
    /// </summary>
    private readonly IScripts _scripts;

    private readonly ICreatureFunctions _creatureFunctions;

    private readonly ITalkActions _talkActions;

    #endregion

    #region Constructors

    public LuaManager(
        ILogger logger,
        ILuaEnvironment luaEnviroment,
        IConfigManager configManager,
        IScripts scripts,
        ICreatureFunctions creatureFunctions,
        ITalkActions talkActions)
    {
        _logger = logger;
        _luaEnviroment = luaEnviroment;
        _configManager = configManager;
        _scripts = scripts;
        _creatureFunctions = creatureFunctions;
        _talkActions = talkActions;
    }

    #endregion

    public ITalkAction GetTalkAction(string name) => _talkActions.GetTalkAction(name);

    public void Start()
    {
        var dir = AppContext.BaseDirectory;

        if (!string.IsNullOrEmpty(ArgManager.GetInstance().ExePath))
            dir = ArgManager.GetInstance().ExePath;

        ModulesLoadHelper(_luaEnviroment.InitState(), "luaEnviroment");

        var luaState = _luaEnviroment.GetLuaState();

        if (luaState.IsNull)
        {
            //Game.DieSafely("Invalid lua state, cannot load lua functions.");
            Console.WriteLine("Invalid lua state, cannot load lua functions.");
        }

        Lua.OpenLibs(luaState);

        //CoreFunctions.Init(L);
        //CreatureFunctions.Init(L);
        //EventFunctions.Init(L);
        //ItemFunctions.Init(L);
        //MapFunctions.Init(L);
        //ZoneFunctions.Init(L);

        LoggerFunctions.Init(luaState);
        ConfigFunctions.Init(luaState);
        GlobalFunctions.Init(luaState);
        TalkActionFunctions.Init(luaState);

        //CreatureFunctions.Init(luaState);
        _creatureFunctions.Init(luaState);

        PlayerFunctions.Init(luaState);

        //GameFunctions.Init(L);
        //CreatureFunctions.Init(L);
        //PlayerFunctions.Init(L);
        //ActionFunctions.Init(L);
        //GlobalEventFunctions.Init(L);
        //CreatureEventsFunctions.Init(L);

        ModulesLoadHelper(_configManager.Load($"{dir}\\config.lua"), $"config.lua");

        ModulesLoadHelper(_luaEnviroment.LoadFile($"{dir}\\DataLuaJit/core.lua", "core.lua"), "core.lua");

        ModulesLoadHelper(_scripts.LoadScripts($"{dir}\\DataLuaJit/scripts", false, false), "/DataLuaJit/scripts");
    }

    #region Private Methods

    private void ModulesLoadHelper(bool loaded, string moduleName)
    {
        _logger.Information($"Loaded {moduleName}");
        if (!loaded)
            _logger.Error(string.Format("Cannot load: {0}", moduleName));
    }

    #endregion
}
