using System.Runtime.Serialization;

namespace Vehicle.Blocks
{
	/// <summary>
	/// The derived ActiveBlock classes will hold the data related to a vehicle's design and simulation,
	/// so it's position (,etc.) and the properties that may have been set in the VehicleEditor and the simulation state.
	/// </summary>
	[DataContract]
	public abstract class ActiveBlock : Block
	{

	}
}
