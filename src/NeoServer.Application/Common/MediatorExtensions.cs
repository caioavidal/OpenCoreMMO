using Mediator;
using NeoServer.Game.Common.Contracts;

namespace NeoServer.Application.Common;

public static class MediatorExtensions
{
    public static void PublishGameEvents(this IMediator mediator, IHasEvent entity)
    {
        foreach (var gameEvent in entity.Events)
        {
            _ = ValueTask.FromResult(mediator.Publish(gameEvent));
        }
    }
}