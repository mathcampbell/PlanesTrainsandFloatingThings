using System;
using System.Runtime.Serialization;

namespace BlockDefinitions.Types
{
	/// <summary>
	/// An Flags Enum to define sides of a single block.
	/// </summary>
	[Flags]
	[DataContract]
	public enum BlockSides : UInt16
	{
		// We need to set the values explicitly, because it defaults to just counting upwards.
		/// <summary>
		/// None shouldn't be used, but exists because a zero value is required for technical reasons.
		/// </summary>
		  None   = 0b_000000
		, Top    = 0b_000001
		, Bottom = 0b_000010
		, Left   = 0b_000100
		, Right  = 0b_001000
		, Front  = 0b_010000
		, Rear   = 0b_100000
		, All    = Top | Bottom | Left | Right | Front | Rear // 0b_111111
	}
}
