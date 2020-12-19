using System;
using System.Runtime.Serialization;

namespace BlockDefinitions.Types
{
	/// <summary>
	/// A Enum to define the shape of a block side,
	/// </summary>
	[DataContract]
	public enum BlockFaceShape : UInt16
	{
		/// <summary>
		/// None shouldn't be used, but exists because a zero value is required for technical reasons.
		/// </summary>
		None = 0,
		Square   = 1,
		Triangle = 2,

	}
}
