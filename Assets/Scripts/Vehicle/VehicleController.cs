#define ShortBlockRecordID

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Assets.Scripts.Vehicle.Blocks;

[DataContract]
public class VehicleController : MonoBehaviour
{
	// This class will be added to the root object of any vehicle added to a world scene.

	// It will need to spawn a GridController to provide the grid of blocks that the root object has, then work out and declare the EnclosedSpaces and ExposedSpaces.
	// These should probably be a separate class tht records the pressure, liquid content, gas content etc
	// It will also need to manage the camera system for third-person mode if a player is controlling the vehicle.

	#region NestedTypes
	[DataContract]
	public struct Orientation
	{
		// Placeholder
		// We can probably get away with an int or enum, but I didn't feel like figuring that out right now.
	}

	[DataContract]
	public struct BlockIDType
	{
		// We could use just a plain UInt16, but if we ever want to change it it'd be a pain to go and do that everywhere it's used

		[DataMember]
		public UInt16 ID;
		// 65536 block types. And in case that is not enough, just change it!

		private BlockIDType(byte b)
		{
			ID = b;
		}

		private BlockIDType(UInt16 b)
		{
			ID = b;
		}

		/// <summary>
		/// Convert from byte
		/// </summary>
		/// <param name="b"></param>
		public static implicit operator BlockIDType(byte b) => new BlockIDType(b);

		/// <summary>
		/// Convert to UInt16
		/// </summary>
		/// <param name="id"></param>
		public static explicit operator UInt16(BlockIDType id) => id.ID;


		/// <summary>
		/// Convert to byte. This could fail if the resulting number does not fit!
		/// </summary>
		/// <param name="id"></param>
		public static explicit operator byte(BlockIDType id) => (byte)id.ID;
	}

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






	public void Awake()
	{
		
	}

	public void Start()
	{
		
	}

	public void FixedUpdate()
	{
		
	}

	public void Update()
	{
		
	}


}
