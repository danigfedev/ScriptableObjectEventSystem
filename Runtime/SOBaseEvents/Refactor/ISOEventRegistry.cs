using System;

namespace SOBaseEvents.Refactor
{
    public interface ISOEventRegistry
    {
        void AddListener(Action listener);
        void RemoveListener(Action listener);
    }
    
    public interface ISOEventRegistry<TEventArg>
    {
        void AddListener(Action<TEventArg> listener);
        void RemoveListener(Action<TEventArg> listener);
    }
    
    //TODO Research more than one Argument support
    //public interface IEventRegistry<TEventArg1, TEventArg2, TEventArg3>
}