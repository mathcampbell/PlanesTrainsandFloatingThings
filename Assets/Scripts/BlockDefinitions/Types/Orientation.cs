using System;
using System.Runtime.Serialization;

namespace BlockDefinitions.Types
{
	/// <summary>
	/// Orientation of a thing in a grid.
	/// </summary>
	[DataContract]
	public struct Orientation
	{
		// Placeholder
		// We can probably get away with an int or enum, but I didn't feel like figuring that out right now.

		// todo: mirroring
		//		* Can be multiple mirrors / complicated
		// todo: do we want the scale / shear hacks like in Stormworks to be possible?
		//		That would mean using a full Transform/Matrix4 though (would need to merge/refactor position and orientation too)


		/// <summary>
		/// Check if <paramref name="other"/> faces exactly opposite to us, but ignore rotations in that axis.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool IsDirectionOpposite(Orientation other)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Check if <paramref name="other"/> faces the same as us, but ignore rotations in that axis.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool IsDirectionSame(Orientation other)
		{
			throw new NotImplementedException();
		}

		public bool IsDirectionSameOrOpposite(Orientation other)
		{
			return IsDirectionSame(other) || IsDirectionOpposite(other);
		}
	}
}
