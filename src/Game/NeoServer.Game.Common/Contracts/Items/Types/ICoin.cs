namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface ICoin : ICumulative
{
    uint Worth { get; }
}