using System;
using System.Diagnostics;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    [DebuggerStepThrough]
    internal sealed class TestStreamId : IStreamIdentity
    {
        public Guid Guid { get; }

        public string Namespace { get; }

        public TestStreamId(Guid id, string streamNamespace)
        {
            Guid = id;
            Namespace = streamNamespace;
        }

        public override bool Equals(object obj)
        {
            var o = obj as TestStreamId;

            return o != null && Guid == o.Guid && Namespace == o.Namespace;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = hash * 7 + Guid.GetHashCode();

            if (Namespace != null)
                hash = hash * 7 + Namespace.GetHashCode();

            return hash;
        }
    }
}