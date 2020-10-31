using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Spells
{
    public class SpellMessage
    {
        private static Dictionary<SpellError, string> messages = new Dictionary<SpellError, string>()
        {
            { SpellError.VocationCannotUse,  "Your vocation cannot use this spell." },
            { SpellError.NotEnoughMana,  "You do not have enough mana." },
        };

        public static string GetMessage(SpellError error) => messages[error];
        
    }
}
