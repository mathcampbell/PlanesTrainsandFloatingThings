//#define ShortBlockRecordID

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using BlockDefinitions;

using Vehicle.BlockBehaviours;
using Vehicle.Blocks;
using System.Collections.Specialized;

using BlockDefinitions.Types;

using DataTypes.Extensions;

using Volume = Vehicle.Volumes.Volume;

namespace Vehicle
{
	/// <summary>
	/// This class holds all vehicle related data, such as the Design, Editor state and Runtime/simulation state.
	/// </summary>
	[DataContract]
	public class VehicleData : IDeserializationCallback
	{
		// todo: is the following still true?
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
		public readonly List<Block> blocks = new List<Block>();


		protected readonly Dictionary<Vector3Int, Block> dictPosition2Block = new Dictionary<Vector3Int, Block>();
		protected IReadOnlyDictionary<Vector3Int, Block> dictPosition2BlockRO; // cannot access non-static thingy in initializer :(
		public IReadOnlyDictionary<Vector3Int, Block> DictPosition2Block => dictPosition2BlockRO;


		private Vector3Int boundsMin;
		private Vector3Int boundsMax;
		private Vector3Int size;

		/// <summary>
		/// The minimum extent of the vehicle
		/// </summary>
		public Vector3Int BoundsMin => boundsMin;

		/// <summary>
		/// The maximum extent of the vehicle
		/// </summary>
		public Vector3Int BoundsMax => boundsMax;

		/// <summary>
		/// The size of the vehicle
		/// </summary>
		public Vector3Int Size => size;


		#endregion Design

		#region Editor

		/// <summary>
		/// Contains the blocks in <see cref="blocks"/> are selectable in the VehicleEditor (to change properties)
		/// </summary>
		public readonly List<Block> propertyBlocks = new List<Block>();


		/// <summary>
		/// The volumes of this vehicle
		/// </summary>
		public readonly List<Volume> volumes = new List<Volume>();

		#endregion Editor

		#region Runtime/Simulation

		/// <summary>
		/// Contains the blocks in <see cref="blocks"/> that need to be updated.
		/// </summary>
		public readonly List<ActiveBlock> activeBlocks = new List<ActiveBlock>();

		#endregion Runtime/Simulation



		public void Initialize()
		{
			// Because C# says so I have to do this here.
			dictPosition2BlockRO = new ReadOnlyDictionary<Vector3Int, Block>(dictPosition2Block);

			boundsMin = Vector3Int.zero;
			boundsMax = Vector3Int.zero;


			foreach (var block in blocks)
			{
				dictPosition2Block.Add(block.position, block);

				boundsMin.UpdateBoundsMin(block.BoundsMin);
				boundsMax.UpdateBoundsMax(block.BoundsMax);

				if (block.IsActiveBlock)
				{
					var ab = block as ActiveBlock;
					if (null == ab) throw new Exception($"Block with {nameof(Block.IsActiveBlock)} == true, but isn't instance of {nameof(ActiveBlock)}");

					activeBlocks.Add(ab);
				}

				if (block.HasProperties)
				{
					propertyBlocks.Add(block);
				}
			}

			size = Mathv.Abs(boundsMax - boundsMin);

			InitializeVolumes();
		}

		public void InitializeVolumes()
		{
			// todo: implement
		}

		/// <inheritdoc />
		public void OnDeserialization(object sender)
		{
			Initialize();
		}

	}
}