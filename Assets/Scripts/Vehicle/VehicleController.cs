using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[DataContract]
public class VehicleController : MonoBehaviour
{
	// This class will be added to the root object of any vehicle added to a world scene.

	// It will need to spawn a GridController to provide the grid of blocks that the root object has, then work out and declare the EnclosedSpaces and ExposedSpaces.
	// These should probably be a separate class tht records the pressure, liquid content, gas content etc
	// It will also need to manage the camera system for third-person mode if a player is controlling the vehicle.

	#region NestedTypes
	[DataContract]
	struct Orientation
	{
		// Placeholder
		// We can probably get away with an int or enum, but I didn't feel like figuring that out right now.
	}

	/// <summary>
	/// BlockRecords are for simple blocks that don't need to store additional information.
	/// </summary>
	[DataContract]
	struct BlockRecord
	{
		[DataMember]
		Vector3Int position;
		[DataMember]
		Orientation orientation;
		[DataMember]
		int blockID;
	}
	#endregion NestedTypes


	[DataMember]
	List<BlockRecord> simpleBlocks = new List<BlockRecord>();
	[DataMember]
	List<Block> complexBlocks = new List<Block>();

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
