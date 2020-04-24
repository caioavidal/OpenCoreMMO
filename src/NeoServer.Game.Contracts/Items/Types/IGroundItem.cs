namespace NeoServer.Game.Contracts.Items
{
    public interface IGroundItem : IItem
    {
        public ushort StepSpeed { get; }
        byte MovementPenalty { get; }
    }
}
