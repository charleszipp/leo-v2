using Phase.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Phase.Tests.Events
{
    [DataContract]
    public class MockCreated : Event
    {
        public MockCreated(Guid mockId, string mockName)
        {
            MockId = mockId;
            MockName = mockName;
        }

        [DataMember]
        public Guid MockId { get; }

        [DataMember]
        public string MockName { get; }
    }
}