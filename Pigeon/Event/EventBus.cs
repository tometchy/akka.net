﻿using Pigeon.Actor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Events
{



    public class EventBus 
    {
        private ConcurrentDictionary<Subscriber, Subscriber> subscribers = new ConcurrentDictionary<Subscriber, Subscriber>();

        public EventBus()
        {

        }
        public void Subscribe(Subscriber subscriber)
        {
            subscribers.TryAdd(subscriber, subscriber);
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            Subscriber tmp;
            subscribers.TryRemove(subscriber, out tmp);
        }

        public void Publish(Event @event)
        {
            foreach(var subscriber in subscribers.Values)
            {
                subscriber.Publish(@event);
            }
        }
    }

    public abstract class Subscriber
    {
        public abstract void Publish(Event @event);
    }

    public class ActorSubscriber : Subscriber
    {
        private ActorRef actor;
        public ActorSubscriber(ActorRef actor)
        {
            this.actor = actor;
        }
        public override void Publish(Event @event)
        {
            actor.Tell(@event);
        }

        public static implicit operator ActorSubscriber(ActorRef actor)
        {
            return new ActorSubscriber(actor);
        }
    }

    public class BlockingCollectionSubscriber : Subscriber
    {
        private BlockingCollection<Event> queue;
        public BlockingCollectionSubscriber(BlockingCollection<Event> queue)
        {
            this.queue = queue;
        }
        public override void Publish(Event @event)
        {
            this.queue.Add(@event);
        }
    }
}
