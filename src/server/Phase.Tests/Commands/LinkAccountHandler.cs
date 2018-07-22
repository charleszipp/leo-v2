using Phase.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Tests.Commands
{
    public class LinkAccountHandler : CommandHandler<LinkAccount>
    {
        public override Task Execute(LinkAccount command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
