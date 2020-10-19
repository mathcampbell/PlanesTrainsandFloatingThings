using System.Runtime.Serialization;

namespace BlockDefinitions.Types
{
	[DataContract]
	public struct Orientation
	{
		// Placeholder
		// We can probably get away with an int or enum, but I didn't feel like figuring that out right now.

		// todo: mirroring
		//		* Can be multiple mirrors / complicated
		// todo: do we want the scale / shear hacks like in Stormworks to be possible?
		//		That would mean using a full Transform/Matrix4 though (would need to merge/refactor position and orientation too)
	}
}
