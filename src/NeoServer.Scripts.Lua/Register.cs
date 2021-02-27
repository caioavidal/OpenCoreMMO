using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NLua;
using NLua.Method;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

            string scriptCode2 = @"
                               
                               function Register(creature) 
                                    creature.OnHear:Add(OnHear)
                               end

                               function OnHear(from, receiver, speechType, message)
                                    print('oi');
                                    if string.match(message, 'teleport') then
                                        from:TeleportTo(from.Location.X + 1, from.Location.Y, 7)
                                    end
Register(creature)
                               end";
            //state.DoString(scriptCode2);

            if (creature is INpc npc)
            {
                state["creature"] = npc;
                state.DoString(scriptCode2);
                //tentar rodar o script acima
                //state.GetFunction("Register").Call();
            }
        }

        // public void teste(dynamic from, dynamic receiver, dynamic speechType, dynamic message) { }

        public void Unsubscribe(ICreature creature)
        {
            throw new NotImplementedException();
        }
    }
}
