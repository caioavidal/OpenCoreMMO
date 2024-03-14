using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Item.Items.Containers.Queries;

public static class FindFirstItemByClientIdQuery
{
    public static (IItem ItemFound, IContainer Container, byte SlotIndex) Find(IContainer onContainer, ushort clientId)
    {
        var containers = new Queue<IContainer>();
        containers.Enqueue(onContainer);

        while (containers.Count > 0)
        {
            containers.TryDequeue(out var container);

            if (container is null) throw new NullReferenceException(nameof(container));
            
            byte slotIndex = 0;
            foreach (var containerItem in container.Items)
            {
                AddInnerContainerToQueue(containerItem, containers);
                if (containerItem.ClientId == clientId) return (containerItem, container, slotIndex);

                slotIndex++;
            }
        }

        return (null, null, 0);
    }

    private static void AddInnerContainerToQueue(IItem containerItem, Queue<IContainer> containers)
    {
        if (containerItem is not IContainer innerContainer) return;
        containers.Enqueue(innerContainer);
    }
}