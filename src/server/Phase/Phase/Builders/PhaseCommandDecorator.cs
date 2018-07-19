using Phase.Interfaces;

namespace Phase.Builders
{
    public static class PhaseCommandFluentDecorator
    {
        public static IPhaseBuilder WithCommandHandler<TCommandHandler, TCommand>(this IPhaseBuilder builder)
            where TCommandHandler : class, IHandleCommand<TCommand>
            where TCommand : ICommand => new PhaseCommandDecorator<TCommandHandler, TCommand>(builder);

        public static IPhaseBuilder WithCommandHandler<TCommandHandler, TCommand, TResult>(this IPhaseBuilder builder)
            where TCommandHandler : class, IHandleCommand<TCommand, TResult>
            where TCommand : ICommand<TResult> => new PhaseCommandDecorator<TCommandHandler, TCommand, TResult>(builder);
    }

    public class PhaseCommandDecorator<TCommandHandler, TCommand> : PhaseDecorator
        where TCommandHandler : class, IHandleCommand<TCommand>
        where TCommand : ICommand
    {
        public PhaseCommandDecorator(IPhaseBuilder builder)
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterCommandHandler<TCommandHandler, TCommand>();
            return rvalue;
        }
    }

    public class PhaseCommandDecorator<TCommandHandler, TCommand, TResult> : PhaseDecorator
        where TCommandHandler : class, IHandleCommand<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {

        public PhaseCommandDecorator(IPhaseBuilder builder)
            : base(builder) { }

        public override Phase Build()
        {
            var rvalue = _builder.Build();
            rvalue.DependencyResolver.RegisterCommandHandler<TCommandHandler, TCommand, TResult>();
            return rvalue;
        }
    }
}