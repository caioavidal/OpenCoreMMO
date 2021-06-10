namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IChargeable
    {
        byte Charges { get; }
        void DecreaseCharges();
    }
}