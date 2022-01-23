using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Spells;

namespace NeoServer.Game.Combat.Spells;

public class SpellList
{
    private static Dictionary<string, ISpell> Spells { get; } = new(StringComparer.InvariantCultureIgnoreCase);

    public static void Add(string words, ISpell spell)
    {
        if (spell is ICommandSpell commandSpell)
        {
            var command = GetCommand(words);
            commandSpell.Params = command.Item2;
            Spells.Add(command.Item1, commandSpell);
        }
        else
        {
            Spells.Add(words, spell);
        }
    }

    public static bool TryGet(string words, out ISpell spell)
    {
        if (words.StartsWith("/"))
        {
            var command = GetCommand(words);
            if (Spells.TryGetValue(command.Item1, out spell) && spell is ICommandSpell commandSpell)
            {
                commandSpell.Params = command.Item2;
                return true;
            }

            return false;
        }

        return Spells.TryGetValue(words, out spell);
    }

    private static (string, object[]) GetCommand(string words)
    {
        var firstWhiteSpace = words.IndexOf(" ");

        if (firstWhiteSpace == -1) return (words, null);

        var command = words.Substring(0, firstWhiteSpace);
        var @params = words.Substring(firstWhiteSpace, words.Length - firstWhiteSpace).Trim().Split(",");

        return (command, @params);
    }
}