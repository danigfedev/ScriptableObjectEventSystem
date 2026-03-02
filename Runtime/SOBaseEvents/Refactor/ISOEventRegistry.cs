using System;

namespace SOBaseEvents.Refactor
{
    //TODO This has to be made with a template if I want to support more Args dynamically
    //public interface IEventRegistry<TEventArg1, TEventArg2, TEventArg3>
    
    public interface ISOEventRegistry<TEventArg>
    {
        void AddListener(Action<TEventArg> listener);
        void RemoveListener(Action<TEventArg> listener);
    }
}