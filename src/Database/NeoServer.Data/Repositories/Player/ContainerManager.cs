using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Data.Contexts;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Data.Repositories.Player;

public class ContainerManager
{
    private readonly NeoContext _neoDbContext;

    public ContainerManager(NeoContext neoDbContext)
    {
        _neoDbContext = neoDbContext;
    }

    public async Task Save(IPlayer player, IContainer container)
    {
        if (Guard.AnyNull(player, container)) return;

        if (container?.Items?.Count == 0) return;

        await using var context = _neoDbContext;

        var containers = new Queue<(IContainer Container, int ParentId)>();
        containers.Enqueue((container, 0));

        while (containers.TryDequeue(out var dequeuedContainer))
        {
            var items = dequeuedContainer.Container.Items;
            if (!items.Any()) continue;

            foreach (var item in items)
            {
                var itemModel = ItemModelParser.ToPlayerItemModel(item);
                if (itemModel is null) continue;

                await context.AddAsync(itemModel);

                if (item is IContainer innerContainer) containers.Enqueue((innerContainer, itemModel.Id));
            }
        }

        await _neoDbContext.SaveChangesAsync();
    }
}