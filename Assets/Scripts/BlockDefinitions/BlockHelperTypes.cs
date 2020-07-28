using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnityEngine;

namespace BlockDefinitions // This file contains multiple items !!
{
	[DataContract]
	public struct Orientation
	{
		// Placeholder
		// We can probably get away with an int or enum, but I didn't feel like figuring that out right now.
	}

	/// <summary>
	/// An Flags Enum to define sides of a single block.
	/// </summary>
	[Flags]
	public enum BlockSides : UInt16
	{
		// We need to set the values explicitly, because it defaults to just counting upwards.
		  None   = 0b_000000
		, Top    = 0b_000001
		, Bottom = 0b_000010
		, Left   = 0b_000100
		, Right  = 0b_001000
		, Front  = 0b_010000
		, Rear   = 0b_100000
		, All    = Top | Bottom | Left | Right | Front | Rear // 0b_111111
	}

	/// <summary>
	/// The type used to hold the BlockID. We may decide to change it later, in which case we should only need to do it here rather than everywhere it's actually used.
	/// </summary>
	[DataContract]
	public struct BlockIDType
	{
		// We could use just a plain UInt16, but if we ever want to change it it'd be a pain to go and do that everywhere it's used

		[DataMember]
		public UInt16 ID;
		// 65536 block types. And in case that is not enough, just change it!

		private BlockIDType(byte b)
		{
			ID = b;
		}

		public BlockIDType(UInt16 b)
		{
			ID = b;
		}

		/// <summary>
		/// Convert from byte
		/// </summary>
		/// <param name="b"></param>
		public static implicit operator BlockIDType(byte b) => new BlockIDType(b);

		/// <summary>
		/// Convert to UInt16
		/// </summary>
		/// <param name="id"></param>
		public static explicit operator UInt16(BlockIDType id) => id.ID;


		/// <summary>
		/// Convert to byte. This could fail if the resulting number does not fit!
		/// </summary>
		/// <param name="id"></param>
		public static explicit operator byte(BlockIDType id) => (byte) id.ID;
	}
}