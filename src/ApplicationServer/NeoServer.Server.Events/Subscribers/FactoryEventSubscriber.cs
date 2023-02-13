using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Server.Events.Subscribers;

public class FactoryEventSubscriber
{
    private readonly IEnumerable<ICreatureEventSubscriber> _creatureEventSubscribers;
    private readonly ICreatureFactory _creatureFactory;

    public FactoryEventSubscriber(ICreatureFactory creatureFactory,
        IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers)
    {
        _creatureFactory = creatureFactory;
        _creatureEventSubscribers = creatureEventSubscribers;
    }

    public void AttachEvents()
    {
        _creatureFactory.OnCreatureCreated += OnCreatureCreated;
    }

    private void OnCreatureCreated(ICreature creature)
    {
        AttachEvents(creature);
    }

    private void AttachEvents(ICreature creature)
    {
        var gameEventSubscriberTypes =
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Select(x => x.FullName)
                .ToHashSet();

        _creatureEventSubscribers.AsParallel().ForAll(subscriber =>
        {
            if (!gameEventSubscriberTypes.Contains(subscriber.GetType().FullName)) return;

            subscriber?.Subscribe(creature);
        });
    }
}