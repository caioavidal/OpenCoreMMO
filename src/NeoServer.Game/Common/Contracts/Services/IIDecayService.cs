using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IDecayService
{
    void Decay(IItem item);
}