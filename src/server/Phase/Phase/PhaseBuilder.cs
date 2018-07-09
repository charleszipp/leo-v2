using Phase.Domains;

namespace Phase
{
    public abstract class PhaseBuilder
    {
        public abstract PhaseContainer Build();
    }

    public class PhaseDefaults : PhaseBuilder
    {
        public override PhaseContainer Build()
        {
            var rvalue = new PhaseContainer();
            //todo: wireup default registrations...


            return rvalue;    
        }
    }
}