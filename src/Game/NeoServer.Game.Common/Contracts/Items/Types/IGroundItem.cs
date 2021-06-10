namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface IGround : IItem
    {
        public ushort StepSpeed { get; }
        byte MovementPenalty { get; }
    }
}