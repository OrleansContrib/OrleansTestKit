using System;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class TestGrainIdentityTests
    {
        [Fact]
        public void LongKeyType()
        {
            var identity = new TestGrainIdentity(2);
            Assert.Equal("2", identity.IdentityString);

            identity = new TestGrainIdentity(3, "ext");
            Assert.Equal("3|ext", identity.IdentityString);
        }

        [Fact]
        public void GuidKeyType()
        {
            var guid = Guid.NewGuid();
            var identity = new TestGrainIdentity(guid);
            Assert.Equal(guid.ToString(), identity.IdentityString);

            guid = Guid.NewGuid();
            identity = new TestGrainIdentity(guid, "ext");
            Assert.Equal($"{guid}|ext", identity.IdentityString);
        }

        [Fact]
        public void StringKeyType()
        {
            var key = "key";
            var identity = new TestGrainIdentity(key);
            Assert.Equal(key, identity.IdentityString);
        }
    }
}
