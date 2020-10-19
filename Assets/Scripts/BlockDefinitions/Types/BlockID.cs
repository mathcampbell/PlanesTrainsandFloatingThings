using System;
using System.Runtime.Serialization;

namespace BlockDefinitions.Types
{
	/// <summary>
	/// The type used to hold the BlockID. We may decide to change it later, in which case we should only need to do it here rather than everywhere it's actually used.
	/// </summary>
	[DataContract]
	public struct BlockID
	{
		// We could use just a plain UInt16, but if we ever want to change it it'd be a pain to go and do that everywhere it's used

		[DataMember]
		public UInt16 ID;
		// 65536 block types. And in case that is not enough, just change it!

		private BlockID(byte b)
		{
			ID = b;
		}

		public BlockID(UInt16 b)
		{
			ID = b;
		}

		public BlockID(Int32 b)
		{
			ID = (UInt16)b;
		}

		public BlockID(BlockID b)
		{
			ID = b.ID;
		}


		public static BlockID operator ++(BlockID a)
		{
			// The compiler does magic to make this work as expected, don't question it.  https://stackoverflow.com/a/19141153
			return new BlockID(a.ID + 1);
		}


		/// <summary>
		/// Convert from byte
		/// </summary>
		/// <param name="b"></param>
		public static implicit operator BlockID(byte b) => new BlockID(b);

		/// <summary>
		/// Convert to UInt16
		/// </summary>
		/// <param name="id"></param>
		public static explicit operator UInt16(BlockID id) => id.ID;


		/// <summary>
		/// Convert to byte. This could fail if the resulting number does not fit!
		/// </summary>
		/// <param name="id"></param>
		public static explicit operator byte(BlockID id) => (byte)id.ID;
	}
}
