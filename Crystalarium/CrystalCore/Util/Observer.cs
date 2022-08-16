using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Util
{
    internal interface Observer
    {
        void Notify(EventGenerator eg);
    }

    public abstract class EventGenerator
    {
        private List<Observer> Observers;

        internal EventGenerator()
        {
            Observers = new List<Observer>();
        }

        internal void Subscribe(Observer ob)
        {
            Observers.Add(ob);
        }

        internal void Unsubscribe(Observer ob)
        {
            Observers.Remove(ob);
        }

        protected void Alert()
        {
            foreach (Observer observer in Observers)
            {
                observer.Notify(this);
            }
        }
    }

}
