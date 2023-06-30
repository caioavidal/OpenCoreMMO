using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Data.Entities;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Data.Repositories.Player;

public class ContainerManager<TEntity> where TEntity : class
{
    private readonly BaseRepository<TEntity> _baseRepository;

    public ContainerManager(BaseRepository<TEntity> baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public async Task Save<TPlayerItemEntity>(IPlayer player, IContainer container) where TPlayerItemEntity : PlayerItemBaseEntity, new()
    {
        if (Guard.AnyNull(player, container)) return;

        if (container?.Items?.Count == 0) return;

        await using var context = _baseRepository.NewDbContext;

        var containers = new Queue<(IContainer Container, int ParentId)>();
        containers.Enqueue((container, 0));

        while (containers.TryDequeue(out var dequeuedContainer))
        {
            var items = dequeuedContainer.Container.Items;
            if (!items.Any()) continue;

            foreach (var item in items)
            {
                var itemModel = ItemModelParser.ToPlayerItemModel<TPlayerItemEntity>(item);
                if (itemModel is null) continue;

                itemModel.PlayerId = (int)player.Id;
                itemModel.ParentId = dequeuedContainer.ParentId;

                await context.AddAsync(itemModel);
                await context.SaveChangesAsync();

                if (item is IContainer innerContainer)
                {
                    containers.Enqueue((innerContainer, itemModel.Id));
                }
            }
        }

        await context.SaveChangesAsync();
    }
}