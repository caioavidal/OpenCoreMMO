using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Standalone;
using System;
using System.IO;

namespace NeoServer.Scripts.Lua
{
    public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
    {
        private readonly ServerConfiguration serverConfiguration;
        private readonly NLua.Lua lua;

        public CreatureEventSubscriber(ServerConfiguration serverConfiguration, NLua.Lua lua)
        {
            this.serverConfiguration = serverConfiguration;
            this.lua = lua;
        }
        public void Subscribe(ICreature creature)
        {
            if (creature is INpc npc && npc.Metadata.IsLuaScript && !string.IsNullOrWhiteSpace(npc.Metadata.Script))
            {
                var script = Path.Combine(serverConfiguration.Data, "npcs", "scripts", npc.Metadata.Script);

                lua.DoFile(script);
                lua.GetFunction("init").Call(creature);
            }
        }

        public void Unsubscribe(ICreature creature)
        {
            throw new NotImplementedException();
        }
    }
}
