namespace Phase
{
    public abstract class PhaseDecorator : PhaseBuilder
    {
        private readonly PhaseBuilder _builder;

        public PhaseDecorator(PhaseBuilder builder)
        {
            _builder = builder;
        }
    }
}