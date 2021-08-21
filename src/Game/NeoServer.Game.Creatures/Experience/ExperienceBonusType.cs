namespace NeoServer.Game.Creatures.Experience
{
    /// <summary>
    /// When multiple experience bonuses of the same type exist, they will be added together.
    /// When multiple experience bonuses of different types exist, they will be multiplied together.
    /// </summary>
    public enum ExperienceBonusType
    {
        Standard = 0,
        Stamina,
        GlobalBonus
    }
}