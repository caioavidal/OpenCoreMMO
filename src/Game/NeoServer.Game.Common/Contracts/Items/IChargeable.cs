namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IChargeable
    {
        ushort Charges { get; }
        void DecreaseCharges();
    }
}