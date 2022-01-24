using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.Inspection;

public interface IInspectionTextBuilder
{
    string Build(IThing thing, bool isClose = false);
    bool IsApplicable(IThing thing);
}