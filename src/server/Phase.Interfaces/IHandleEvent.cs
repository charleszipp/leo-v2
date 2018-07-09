namespace Phase.Interfaces
{
    public interface IHandleEvent<in TEvent>
        where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}