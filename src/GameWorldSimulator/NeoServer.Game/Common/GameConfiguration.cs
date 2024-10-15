namespace NeoServer.Game.Common;

public record GameConfiguration(
    decimal ExperienceRate = 1,
    decimal LootRate = 1,
    Dictionary<string, double> SkillsRate = null,
    CombatConfiguration Combat = null,
    bool InfiniteRune = false
);

public sealed record CombatConfiguration(
    int MaxAttacksPerTurn = 5,
    bool InfiniteAmmo = false,
    bool InfiniteThrowingWeapon = false
);