using System;
using Owin;

namespace Leo.Core.Owin
{
    public abstract class AppBuilder
    {
        public abstract void Configure(IAppBuilder app);
    }

    public class DefaultAppBuilder : AppBuilder
    {
        public override void Configure(IAppBuilder app)
        {
        }
    }
}
