using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Scripts.Npcs
{
    public class DialogKeywordReplacement
    {
        public DialogKeywordReplacement()
        {

        }

        public List<Func<IPlayer, string, string>> Replacements { get; } = new List<Func<IPlayer, string, string>>();

        public IEnumerable<Func<IPlayer, string, string>> ReplaceFunctions
        {
            get
            {
                yield return (IPlayer p, string m) => m.Replace("|PLAYERNAME|", p.Name);
            }
        }
    }
}
