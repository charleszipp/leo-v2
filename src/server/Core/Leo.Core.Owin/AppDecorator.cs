namespace Leo.Core.Owin
{
    public abstract class AppDecorator : AppBuilder
    {
        protected readonly AppBuilder _build;

        public AppDecorator(AppBuilder build)
        {
            _build = build;
        }
    }
}