using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoServer.Scripts.Luaa
{
    public class Register
    {
    }

    public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
    {
        public void Subscribe(ICreature creature)
        {
            Lua state = new Lua();

           
            if (creature is INpc npc)
            {
                state.LoadCLRPackage();
                string scriptCode2 = @"
                                function replace (npc, to)
                                    npc:Follow(to);
                                end";
                state.DoString(scriptCode2);
                var replace = state["replace"] as LuaFunction;
            
                npc.OnSay += (a,b,c,d) =>
                {
                  
                    replace.Call(a,d);
                };

            }

          
        }

    

        public void Unsubscribe(ICreature creature)
        {
            throw new NotImplementedException();
        }
    }
}
