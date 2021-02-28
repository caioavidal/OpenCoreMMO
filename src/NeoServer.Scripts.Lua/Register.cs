using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Standalone;
using NLua;
using System;
using System.IO;

namespace NeoServer.Scripts.Luaa
{
    public class Init { }
    public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
    {
        private readonly ServerConfiguration serverConfiguration;

        public CreatureEventSubscriber(ServerConfiguration serverConfiguration)
        {
            this.serverConfiguration = serverConfiguration;
        }
        public void Subscribe(ICreature creature)
        {
            if (creature is INpc npc && npc.Metadata.IsLuaScript && !string.IsNullOrWhiteSpace(npc.Metadata.Script))
            {
                Lua state = new Lua();
                
                    var script = Path.Combine(serverConfiguration.Data, "npcs", "scripts", npc.Metadata.Script);
                state["creature"] = npc;

                state.DoFile(script);
            }
        }

        public void Unsubscribe(ICreature creature)
        {
            throw new NotImplementedException();
        }
    }
}
