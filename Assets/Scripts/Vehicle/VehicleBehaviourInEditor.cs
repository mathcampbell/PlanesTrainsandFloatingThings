using System;

using DataTypes;

using UnityEngine;

using Vehicle.Blocks;

namespace Vehicle
{
	/// <summary>
	/// Behaviour that manages VehicleEditor specific behaviours of a vehicle.
	/// </summary>
	[RequireComponent(typeof(VehicleBehaviour))]
	[RequireComponent(typeof(MeshCollider))]

	// We must run afterwards, and Unity isn't smart enough to infer that from RequireComponent, so we have to do it manually.
	[DefaultExecutionOrder(defaultExecutionOrder)]
	public class VehicleBehaviourInEditor : M0noBehaviour
	{
		public const int defaultExecutionOrder = VehicleBehaviour.defaultExecutionOrder + 1;

		[SerializeField]
		private VehicleBehaviour mainBehaviour;

		[SerializeField]
		private MeshCollider mainCollider;

		[NonSerialized] // todo: for hot-reload: fetch it from mainBehaviour.
		private VehicleData vehicleData;


		/// <inheritdoc />
		protected override void OnStart()
		{
			mainBehaviour = GetComponent<VehicleBehaviour>();
			mainCollider = GetComponent<MeshCollider>();
			vehicleData = mainBehaviour.vehicleData;
		}




		public void AddBlock(Block block)
		{
			throw new NotImplementedException();
		}


		public void RemoveBlock(Block block)
		{
			throw new NotImplementedException();
		}

		public void RemoveBlock(Vector3Int gridPosition)
		{
			if (vehicleData.DictPosition2Block.TryGetValue(gridPosition, out var block))
			{
				RemoveBlock(block);
			}
		}


		/// <summary>
		/// Raycast against this vehicle in VehicleEditor.
		/// Specify layers to select what will be hit (connectableSides, SealedSides, voxels).
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="layers"></param>
		/// <param name="hit"></param>
		/// <returns></returns>
		public bool EditorRayCast(Ray ray, LayerMask layers,
		                          out RaycastHit hit,
		                          out Vector3Int vehicleGrid,
		                          out Vector3 blockGrid,
		                          out Block block
		                          // todo: something to Identify the BockFace that was hit, and what kind of Face that is.
		)
		{
			// todo: this should raycast against the vehicle for adding/removing blocks, painting that sort of stuff.
			// Probably the easiest way:
			// Use the generated mesh, and let unity do the heavy lifting.
			// Then use the hit Unity gave and look where that is in our data structure, and return the appropriate thing.


			if (mainCollider.Raycast(ray, out var colliderHit, float.MaxValue))
			{
				throw new NotImplementedException();
			}
			else
			{
				hit = default;
				vehicleGrid = default;
				blockGrid = default;
				block = null;
				return false;
			}
		}
	}
}
