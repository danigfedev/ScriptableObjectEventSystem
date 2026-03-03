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
    
    public interface ISOEventRegistry<TEventArg1, TEventArg2>
    {
        void AddListener(Action<TEventArg1, TEventArg2> listener);
        void RemoveListener(Action<TEventArg1, TEventArg2> listener);
    }
    
    public interface ISOEventRegistry<TEventArg1, TEventArg2, TEventArg3>
    {
        void AddListener(Action<TEventArg1, TEventArg2, TEventArg3> listener);
        void RemoveListener(Action<TEventArg1, TEventArg2, TEventArg3> listener);
    }
}