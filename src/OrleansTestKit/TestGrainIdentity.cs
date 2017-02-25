using System;
using Orleans.Core;

namespace Orleans.TestKit
{
    public sealed class TestGrainIdentity : IGrainIdentity
    {
        private enum KeyType
        {
            String,
            Guid,
            Long
        }

        private readonly KeyType _keyType;

        public Guid PrimaryKey { get; }

        public long PrimaryKeyLong { get; }

        public string PrimaryKeyString { get; }

        public string IdentityString
        {
            get
            {
                switch (_keyType)
                {
                    case KeyType.String:
                        return PrimaryKeyString;
                    case KeyType.Guid:
                        return PrimaryKey.ToString();
                    case KeyType.Long:
                        return PrimaryKeyLong.ToString();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public TestGrainIdentity(Guid id)
        {
            PrimaryKey = id;
            _keyType = KeyType.Guid;
        }

        public TestGrainIdentity(long id)
        {
            PrimaryKeyLong = id;
            _keyType = KeyType.Long;
        }

        public TestGrainIdentity(string id)
        {
            PrimaryKeyString = id;
            _keyType = KeyType.String;
        }

        public long GetPrimaryKeyLong(out string keyExt)
        {
            throw new NotImplementedException();
        }

        public Guid GetPrimaryKey(out string keyExt)
        {
            throw new NotImplementedException();
        }
    }
}