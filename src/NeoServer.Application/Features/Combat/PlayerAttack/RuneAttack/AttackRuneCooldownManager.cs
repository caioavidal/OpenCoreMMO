using NeoServer.Application.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Creatures.Structs;

namespace NeoServer.Application.Features.Combat.PlayerAttack.RuneAttack;

public class AttackRuneCooldownManager : ISingleton
{
    private readonly Dictionary<uint, CooldownTime> _cooldowns = new();

    public bool Start(IPlayer player, IAttackRune rune)
    {
        if (Expired(player)) _cooldowns.Remove(player.Id);
        return _cooldowns.TryAdd(player.Id, new CooldownTime(DateTime.Now, rune.CooldownTime));
    }

    public bool Expired(IPlayer player)
    {
        if (_cooldowns.TryGetValue(player.Id, out var cooldown)) return cooldown.Expired;
        return true;
    }
}