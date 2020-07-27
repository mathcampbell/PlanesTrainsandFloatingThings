//#define ShortBlockRecordID

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


using BlockDefinitions;
using Vehicle.Blocks;

namespace Vehicle
{

	[DataContract]
	public class VehicleController : MonoBehaviour
	{
		// This class will be added to the root object of any vehicle added to a world scene.

		// It will need to spawn a GridController to provide the grid of blocks that the root object has, then work out and declare the EnclosedSpaces and ExposedSpaces.
		// These should probably be a separate class tht records the pressure, liquid content, gas content etc
		// It will also need to manage the camera system for third-person mode if a player is controlling the vehicle.

		#region NestedTypes

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
			public BlockIDType BlockID;
#endif

			/// <summary>
			/// Fetch the <see cref="BlockDefinition"/> for this block.
			/// </summary>
			public BlockDefinition Definition => BlockDefinition.Definitions[BlockID];
		}

		#endregion BlockRecord Optimalization (Can be ignored for now)

		#endregion NestedTypes

		// Not implemented, but if commented out would break with renames and other refactorings.
		[DataMember]
		List<BlockRecord> simpleBlocks = new List<BlockRecord>();


		[DataMember]
		List<Block> blocks = new List<Block>();

		[DataMember]
		List<ActiveBlock> complexBlocks = new List<ActiveBlock>();

		public void Test()
		{
			var foo = new BinaryFormatter();
		}


		private GameObject myGameObject;






		public void Awake() { }

		public void Start() { }

		public void FixedUpdate() { }

		public void Update() { }


	}
}