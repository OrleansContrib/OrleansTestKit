// using System;
// using System.Diagnostics;
// using System.Diagnostics.CodeAnalysis;
// using System.Globalization;
// using Orleans.Runtime;
//
// namespace Orleans.TestKit
// {
//     [DebuggerStepThrough]
//     public sealed class TestGrainIdentity :
//         IGrainIdentity
//     {
//         private enum KeyType
//         {
//             String,
//             Guid,
//             Long,
//             GuidCompound,
//             LongCompound
//         }
//
//         private readonly KeyType _keyType;
//
//         [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
//         public Guid PrimaryKey { get; }
//
//         [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
//         public long PrimaryKeyLong { get; }
//
//         public string PrimaryKeyString { get; }
//
//         public string KeyExtension { get; }
//
//         public string IdentityString =>
//             _keyType switch
//             {
//                 KeyType.String => PrimaryKeyString,
//                 KeyType.Guid => PrimaryKey.ToString(),
//                 KeyType.Long => PrimaryKeyLong.ToString(CultureInfo.InvariantCulture),
//                 KeyType.GuidCompound => $"{PrimaryKey}|{KeyExtension}",
//                 KeyType.LongCompound => $"{PrimaryKeyLong.ToString(CultureInfo.InvariantCulture)}|{KeyExtension}",
//                 _ => throw new InvalidOperationException(),
//             };
//
//         [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
//         public bool IsClient => throw new NotImplementedException();
//
//         [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
//         public int TypeCode => throw new NotImplementedException();
//
//         public TestGrainIdentity(Guid id, string keyExtension = null)
//         {
//             PrimaryKey = id;
//             _keyType = keyExtension != null ? KeyType.GuidCompound : KeyType.Guid;
//             KeyExtension = keyExtension;
//         }
//
//         public TestGrainIdentity(long id, string keyExtension = null)
//         {
//             PrimaryKeyLong = id;
//             _keyType = keyExtension != null ? KeyType.LongCompound : KeyType.Long;
//             KeyExtension = keyExtension;
//         }
//
//         public TestGrainIdentity(string id)
//         {
//             PrimaryKeyString = id ?? throw new ArgumentNullException(nameof(id));
//             _keyType = KeyType.String;
//         }
//
//         public long GetPrimaryKeyLong(out string keyExt)
//         {
//             keyExt = KeyExtension;
//             return PrimaryKeyLong;
//         }
//
//         public Guid GetPrimaryKey(out string keyExt)
//         {
//             keyExt = KeyExtension;
//             return PrimaryKey;
//         }
//
//         public uint GetUniformHashCode() =>
//             throw new NotImplementedException();
//     }
// }
