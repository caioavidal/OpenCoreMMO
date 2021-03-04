using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts;
using NeoServer.Server.Standalone;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Lua
{
    public class LuaGlobalRegister
    {
        private readonly IGameServer gameServer;
        private readonly IItemFactory itemFactory;
        private readonly ICreatureFactory creatureFactory;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Logger logger;
        private readonly NLua.Lua lua;

        public LuaGlobalRegister(IGameServer gameServer, IItemFactory itemFactory, ICreatureFactory creatureFactory, NLua.Lua lua, ServerConfiguration serverConfiguration, Logger logger)
        {
            this.gameServer = gameServer;
            this.itemFactory = itemFactory;
            this.creatureFactory = creatureFactory;
            this.lua = lua;
            this.serverConfiguration = serverConfiguration;
            this.logger = logger;
        }

        public void Register()
        {
            lua.LoadCLRPackage();


            lua["gameServer"] = gameServer;
            lua["map"] = gameServer.Map;
            lua["itemFactory"] = itemFactory;
            lua["creatureFactory"] = creatureFactory;
            lua["load"] = new Action<string>((path) => DoFile(path));
            lua["logger"] = logger;

            ExecuteMainFiles();

            logger.Information("{Lua} scripts loaded!", "Lua");
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
            {
                lua.DoFile(file);
            }
        }

        public void DoFile(string luaPath)
        {
            lua.DoFile(Path.Combine(serverConfiguration.Data, luaPath));
        }
    }
}
