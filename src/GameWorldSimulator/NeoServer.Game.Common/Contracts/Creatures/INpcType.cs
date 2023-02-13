using System.Collections.Generic;

namespace NeoServer.Game.Common.Contracts.Creatures;

public interface INpcType : ICreatureType
{
    public string Script { get; set; }
    public IDialog[] Dialogs { get; init; }
    public IDictionary<string, dynamic> CustomAttributes { get; }
    bool IsLuaScript { get; }
    string[] Marketings { init; get; }
    uint WalkInterval { get; set; }
    bool IsCSharpScript { get; }
}

public interface IDialog
{
    public string[] OnWords { get; init; }
    public string[] Answers { get; init; }
    public string Action { get; init; }
    public bool End { get; init; }
    public IDialog[] Then { get; init; }
    string StoreAt { get; init; }
    byte Back { get; init; }
}