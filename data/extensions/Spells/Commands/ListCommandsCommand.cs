using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Bases;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers;
using Newtonsoft.Json;

namespace NeoServer.Extensions.Spells.Commands;

public class ListCommandsCommand : CommandSpell
{
    private const string SpellType = "command";

    public override bool OnCast(ICombatActor actor, string command, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;

        if (actor is not IPlayer player) return false;

        var spells = LoadSpells();
        var text = BuildTextFromSpells(spells, command);

        TextWindow window = new TextWindow(player.Location, text);
        player.Read(window);

        return true;
    }

    private string BuildTextFromSpells(List<IDictionary<string, object>> spells, string command)
    {
        List<string> lines = new List<string>();
        foreach (var spell in spells)
        {
            var (words, name) = ExtractSpellAttributes(spell);

            if (words == command)
                continue;

            lines.Add($"{words} {name}");
        }
        return string.Join(Environment.NewLine + Environment.NewLine, lines);
    }

    private (string, string) ExtractSpellAttributes(IDictionary<string, object> spell)
    {
        if (spell is null || !spell.ContainsKey("type") || spell["type"]?.ToString() != SpellType)
        {
            return (String.Empty, String.Empty);
        }

        string words = spell["words"].ToString();
        string name = spell["name"].ToString();

        return (words, name);
    }

    private List<IDictionary<string, object>> LoadSpells()
    {
        var serverConfiguration = IoC.GetInstance<ServerConfiguration>();
        var path = Path.Combine(serverConfiguration.Data, "spells", "spells.json");
        var jsonString = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(jsonString)?.ToList() ??
                     new List<IDictionary<string, object>>(0);
    }

    private class TextWindow : BaseItem, IReadable
    {
        public TextWindow(Location location, string text) : base(
            new ItemType(), location)
        {
            Text = text?.ToString(CultureInfo.InvariantCulture);
        }

        public string Text { get; private set; }
        public ushort MaxLength => (ushort)(Text?.Length ?? 0);
        public bool CanWrite => false;
        public string WrittenBy { get; private set; }
        public DateTime? WrittenOn { get; set; }

        public Result Write(string text, IPlayer writtenBy)
        {
            if (!CanWrite) return Result.Fail(InvalidOperation.NotPossible);

            if (text.IsNull()) return Result.Success;

            if (text.Length > MaxLength) return Result.Fail(InvalidOperation.NotPossible);

            Text = text;
            return Result.Success;
        }
    }
}