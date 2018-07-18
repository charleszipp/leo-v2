using Phase.Interfaces;
using System;

namespace Phase.Tests.Commands
{
    public class CreateMock : ICommand
    {
        public CreateMock(Guid mockId, string mockName)
        {
            MockId = mockId;
            MockName = mockName;
        }

        public Guid MockId { get; }

        public string MockName { get; }
    }
}