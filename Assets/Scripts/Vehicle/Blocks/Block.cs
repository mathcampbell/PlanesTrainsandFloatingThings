using System;
using System.Collections;
using System.Collections.Generic;


using BlockDefinitions;

using UnityEngine;

namespace Vehicle.Blocks
{
	/// <summary>
	/// The block class, and derived classes will hold the data related to a vehicle's design,
	/// so it's position (,etc.) and the properties that may have been set in the VehicleEditor.
	/// </summary>
	public class Block : MonoBehaviour // Todo: Should not be MonoBehaviour. But there should be a representaiton for the block with monobehaviour so ...
	{
		#region Definition
		// Shortcuts to the Definition of this block.

		[NonSerialized]
		protected BlockDefinition myBlockDefinition;

		[SerializeField]
		public readonly BlockID blockID;

		public bool IsSingleCubeBlock => myBlockDefinition.IsSingleCubeBlock;

		public bool IsMultiCubeBlock => myBlockDefinition.IsMultiCubeBlock;

		public bool IsActiveBlock => myBlockDefinition.IsActiveBlock;

		public float Mass => myBlockDefinition.Mass;

		public string Name => myBlockDefinition.Name;

		public string Description => myBlockDefinition.Description;
		#endregion Definition


		#region Design
		// How this block relates to the vehicle.

		public Vector3Int position;

		public Orientation orientation;


		#endregion

		
	}
}