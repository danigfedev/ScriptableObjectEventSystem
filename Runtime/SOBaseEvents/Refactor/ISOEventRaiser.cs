namespace SOBaseEvents.Refactor
{
    //TODO This has to be made with a template if I want to support more Args dynamically
    
    public interface ISOEventRaiser<TEventArg>
    {
        void RaiseEvent(TEventArg arg);
    }
}