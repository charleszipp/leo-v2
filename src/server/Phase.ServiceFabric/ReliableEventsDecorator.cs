using Microsoft.ServiceFabric.Data;

namespace Phase.ServiceFabric
{
    public class ReliableEventsDecorator : PhaseDecorator
    {
        private readonly IReliableStateManager _states;

        public ReliableEventsDecorator(PhaseBuilder builder, IReliableStateManager states) 
            : base(builder)
        {
            _states = states;
        }

        public override PhaseContainer Build()
        {
            var rvalue = _builder.Build();

            rvalue.EventsProvider = rvalue.EventsProvider.AdaptTo((ep) => new ReliableEventsProvider(_states, ep));

            return rvalue;
        }
    }
}
