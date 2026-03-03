namespace SOBaseEvents.Refactor
{
    public interface ISOEventRaiser
    {
        void RaiseEvent();
    }
    
    public interface ISOEventRaiser<TEventArg>
    {
        void RaiseEvent(TEventArg arg);
    }
    
    //TODO Research more than one Argument support
}