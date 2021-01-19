namespace NeoServer.Game.Contracts.Items
{
    public interface IGround : IItem
    {
        public ushort StepSpeed { get; }
        byte MovementPenalty { get; }
    }
}
