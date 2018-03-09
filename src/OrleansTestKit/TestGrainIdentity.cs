using System;
using System.Diagnostics;
using Orleans.Core;

namespace Orleans.TestKit
{
    [DebuggerStepThrough]
    public sealed class TestGrainIdentity : IGrainIdentity
    {
        private enum KeyType
        {
            String,
            Guid,
            Long,
            GuidCompound,
            LongCompound
        }

        private readonly KeyType _keyType;

        public Guid PrimaryKey { get; }

        public long PrimaryKeyLong { get; }

        public string PrimaryKeyString { get; }

        public string KeyExtension { get; }

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
                    case KeyType.GuidCompound:
                        return $"{PrimaryKey}|{KeyExtension}";
                    case KeyType.LongCompound:
                        return $"{PrimaryKeyLong}|{KeyExtension}";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsClient { get { throw new NotImplementedException(); } }

        public int TypeCode { get { throw new NotImplementedException(); } }

        public TestGrainIdentity(Guid id, string keyExtension = null)
        {
            PrimaryKey = id;
            _keyType = KeyType.Guid;
            KeyExtension = keyExtension;
        }

        public TestGrainIdentity(long id, string keyExtension = null)
        {
            PrimaryKeyLong = id;
            _keyType = KeyType.Long;
            KeyExtension = keyExtension;
        }

        public TestGrainIdentity(string id)
        {
            PrimaryKeyString = id;
            _keyType = KeyType.String;
        }

        public long GetPrimaryKeyLong(out string keyExt)
        {
            keyExt = KeyExtension;
            return PrimaryKeyLong;
        }

        public Guid GetPrimaryKey(out string keyExt)
        {
            keyExt = KeyExtension;
            return PrimaryKey;
        }

        public uint GetUniformHashCode() 
        {
            throw new NotImplementedException();
        }
    }
}
