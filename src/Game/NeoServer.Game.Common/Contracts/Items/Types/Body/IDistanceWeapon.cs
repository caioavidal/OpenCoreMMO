using System.Text;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface IDistanceWeapon : IWeapon
    {
        byte ExtraAttack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);
        byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.HitChance);
        byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);
        
    }
}