using System;
using System.Collections.Generic;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Scripts.Npcs
{
    public class DialogKeywordReplacement
    {
        public List<Func<IPlayer, string, string>> Replacements { get; } = new();

        public IEnumerable<Func<IPlayer, string, string>> ReplaceFunctions
        {
            get { yield return (p, m) => m.Replace("|PLAYERNAME|", p.Name); }
        }
    }
}