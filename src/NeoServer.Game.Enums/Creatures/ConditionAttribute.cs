using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Creatures.Players
{
    public enum ConditionAttribute
    {
		Type = 1,
		Id,
		Ticks,
		Healthticks,
		Healthgain,
		Manaticks,
		Managain,
		Delayed,
		Owner,
		Intervaldata,
		Speeddelta,
		Formula_mina,
		Formula_minb,
		Formula_maxa,
		Formula_maxb,
		Lightcolor,
		Lightlevel,
		Lightticks,
		Lightinterval,
		Soulticks,
		Soulgain,
		Skills,
		Stats,
		Outfit,
		Perioddamage,
		Isbuff,
		Subid,
		End = 254,  //reserved for serialization

	}
}
