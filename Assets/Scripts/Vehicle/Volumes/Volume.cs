using System.Collections.Generic;
using System.Runtime.Serialization;

using UnityEngine;

using Vehicle.Blocks;

namespace Vehicle.Volumes
{
	/// <summary>
	/// A volume is a enclosed space in a vehicle, that can contain (a) fluid(s) or gas(es).
	/// </summary>
	public class Volume
	{
		/// <summary>
		/// The vehicle we belong to
		/// </summary>
		public VehicleData parent;

		/// <summary>
		/// Neighbouring volumes, such as those connected via a door.
		/// </summary>
		public List<Volume> neighbours; // Set during Init?

		/// <summary>
		/// The blocks that make up the walls of this volume.
		/// </summary>
		public List<Block> walls;

		/// <summary>
		/// Damaged blocks that are leaking.
		/// </summary>
		public List<Block> leaks;

		/// <summary>
		/// The capacity of this volume.
		/// </summary>
		public float capacity;

		/// <summary>
		/// The pressure in the volume.
		/// </summary>
		public float pressure;

		/// <summary>
		/// The contents of this volume.
		/// </summary>
		public List<VolumeFluid> contents;

	}
}
