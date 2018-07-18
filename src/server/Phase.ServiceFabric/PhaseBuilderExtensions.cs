using Microsoft.ServiceFabric.Data;

namespace Phase.ServiceFabric
{
    public static class PhaseBuilderExtensions
    {
        public static PhaseBuilder WithServiceFabric(this PhaseBuilder builder, IReliableStateManager states)
        {
            return new ReliableEventsDecorator(builder, states);
        }
    }
}
