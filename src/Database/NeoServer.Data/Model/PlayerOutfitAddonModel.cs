using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Data.Model;

public class PlayerOutfitAddonModel
{
    public PlayerModel Player { get; set; }
    public int PlayerId { get; set; }
    public int LookType { get; set; }
    public OutfitAddon AddonLevel { get; set; } = OutfitAddon.None;
}