//#define ShortBlockRecordID

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using BlockDefinitions;

using Vehicle.BlockBehaviours;
using Vehicle.Blocks;
using System.Collections.Specialized;

namespace Vehicle
{
	/// <summary>
	/// This class holds all vehicle related data, such as the Design, Editor state and Runtime/simulation state.
	/// </summary>
	[DataContract]
	public class VehicleData : IDeserializationCallback
	{
		// This class will be added to the root object of any vehicle added to a world scene.

		// It will need to spawn a GridController to provide the grid of blocks that the root object has,
		// then work out and declare the EnclosedSpaces and ExposedSpaces.
		// These should probably be a separate class tht records the pressure, liquid content, gas content etc
		// It will also need to manage the camera system for third-person mode if a player is controlling the vehicle.


		#region Design

		#region BlockRecord Optimalization (Can be ignored for now)
		/// <summary>
		/// BlockRecords are for simple blocks that don't need to store additional information.
		/// </summary>
		[DataContract]
		public struct BlockRecord
		{
			// TODO: Keeping the size of this struct as small as possible will help performance.
			// There will be MANY instances of this.


			// TODO: Int16 should suffice, maybe just: Int16 x, Int16 y, Int16 z? (Vector3Int16 does not exist)
			[DataMember]
			public Vector3Int Position;

			[DataMember]
			public Orientation Orientation;

			[DataMember]
#if ShortBlockRecordID
			public byte BlockID; // We can probably get away with this, since BlockRecord will only hold Trivial blocks.
#else
			public BlockID BlockID;
#endif

			/// <summary>
			/// Fetch the <see cref="BlockDefinition"/> for this block.
			/// </summary>
			public BlockDefinition Definition => BlockDefinition.Definitions[BlockID];
		}
		// Not implemented, but if commented out would break with renames and other refactorings.
		[DataMember]
		List<BlockRecord> simpleBlocks = new List<BlockRecord>();
		#endregion BlockRecord Optimalization (Can be ignored for now)

		[DataMember]
		List<Block> blocks = new List<Block>();


		#endregion Design

		#region Editor


		#endregion Editor

		#region Runtime/Simulation

		[DataMember]
		List<BlockBehaviour> runtimeBlocks = null;
	
		#endregion Runtime/Simulation

		private GameObject myGameObject;



		/// <inheritdoc />
		public void OnDeserialization(object sender)
		{
			//throw new NotImplementedException();
		}

		public Vector3 GetBounds()
		{
			// This needs to return the bounds of the entire vehicle as a Vector3 for the GridController class.
			// For now, implemented as a holding point.
			Vector3 bounds = new Vector3(1, 1, 1);
			return bounds;
		}
	}
}