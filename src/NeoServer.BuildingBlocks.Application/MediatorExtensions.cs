using Mediator;
using NeoServer.Game.Common.Contracts;

namespace NeoServer.BuildingBlocks.Application;

public static class MediatorExtensions
{
    public static void PublishGameEvents(this IMediator mediator, params IHasEvent[] entities)
    {
        foreach (var entity in entities)
            while (entity.Events.TryDequeue(out var gameEvent))
                _ = ValueTask.FromResult(mediator.Publish(gameEvent));
    }
}