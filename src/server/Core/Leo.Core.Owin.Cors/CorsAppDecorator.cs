using Owin;

namespace Leo.Core.Owin.Cors
{
    public class CorsAppDecorator : AppDecorator
    {
        public CorsAppDecorator(AppBuilder build)
            : base(build)
        {
        }

        public override void Configure(IAppBuilder app)
        {
            _build.Configure(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}