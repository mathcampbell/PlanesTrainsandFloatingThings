using System;
using System.Runtime.Serialization;

namespace BlockDefinitions.Types
{
	[DataContract]
	public enum BlockFaceShape : UInt16
	{
		None     = 0,
		Square   = 1,
		Triangle = 2,

	}
}
