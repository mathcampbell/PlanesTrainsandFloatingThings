using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using DataTypes;

using Tools;

using Vehicle.Blocks;


namespace Vehicle
{
	/// <summary>
	/// The behaviour that manages the main vehicle's GameObject through an instance of <see cref="VehicleData"/>.
	/// </summary>
	[DefaultExecutionOrder(defaultExecutionOrder)]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class VehicleBehaviour : M0noBehaviour
	{
		public const int defaultExecutionOrder = 0;

		[NonSerialized] // todo: we probably should in some way to make hot-reload work, but Unity is too dumb to do it properly.
		public VehicleData vehicleData;


		public Material TempMaterial;

		private MeshFilter meshFilter;
		private new MeshRenderer renderer; // hide obsolete renderer in MonoBehaviour

		private Mesh mainMesh;


		/// <inheritdoc />
		protected override void OnStart()
		{
			meshFilter = GetComponent<MeshFilter>();
			renderer = GetComponent<MeshRenderer>();

			if (null == vehicleData) throw new Exception($"{nameof(VehicleBehaviour)} with null {nameof(VehicleData)}");

			vehicleData.Initialize();

			mainMesh = MeshCombiner.CombineMeshes_Dumb(MeshCombinerData().ToList());
			meshFilter.sharedMesh = mainMesh;

			renderer.materials = new Material[mainMesh.subMeshCount];
			for (int i = 0; i < renderer.materials.Length; i++)
			{
				renderer.materials[i] = TempMaterial;
			}
		}



		void FixedUpdate()
		{
			vehicleData.UnityFixedUpdate();
		}

		void Update()
		{
			vehicleData.UnityUpdate();
		}




		private IEnumerable<MeshCombiner.DataItem> MeshCombinerData()
		{
			foreach (var block in vehicleData.blocks)
			{
				if (null != block.myBlockDefinition.Mesh)
				{
					var pos = (Vector3) block.position * Block.DesignToWorldScale;
					var rot = Quaternion.identity; // todo: implement properly
					var scl = Vector3.one;

					var transform = Matrix4x4.Rotate(rot) * Matrix4x4.Scale(scl) * Matrix4x4.Translate(pos);

					yield return new MeshCombiner.DataItem(block.myBlockDefinition.Mesh, transform);
				}
				else
				{
					// todo: implement properly (use BlockFaces)
				}
			}
		}

	}
}
