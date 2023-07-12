using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts;

public interface ICooldownList
{
    /// <summary>
    ///     Add cooldown
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration">milliseconds</param>
    bool Start(CooldownType type, int duration);

    bool Start(string spell, int duration);
    void Add(string spell, int duration);
    bool Expired(CooldownType type);
    bool Expired(string spell);
    void RestartCoolDown(CooldownType type, int duration);
}