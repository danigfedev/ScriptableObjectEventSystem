namespace SOBaseEvents
{
    public interface ISOEventRaiser
    {
        void RaiseEvent();
    }
    
    public interface ISOEventRaiser<TEventArg>
    {
        void RaiseEvent(TEventArg arg);
    }
    
    public interface ISOEventRaiser<TEventArg1, TEventArg2>
    {
        void RaiseEvent(TEventArg1 arg1, TEventArg2 arg2);
    }
    
    public interface ISOEventRaiser<TEventArg1, TEventArg2, TEventArg3>
    {
        void RaiseEvent(TEventArg1 arg1, TEventArg2 arg2, TEventArg3 arg3);
    }
}