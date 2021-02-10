namespace NeoServer.Game.Contracts.Items
{
    public interface IChargeable
    {
        byte Charges { get; }
        void DecreaseCharges();

    }
}
