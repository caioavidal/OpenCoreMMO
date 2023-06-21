using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Items.Items.Containers.Container.Queries;

internal static class FindRootParentQuery
{
    public static IThing Find(IContainer container)
    {
        IThing root = container;
        while (root is IContainer { Parent: not null } parentContainer) root = parentContainer.Parent;
        return root;
    }
}