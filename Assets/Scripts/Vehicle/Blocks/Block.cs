using System;
using System.Collections;
using System.Collections.Generic;


using BlockDefinitions;

using UnityEngine;

namespace Vehicle.Blocks
{

	public class Block : MonoBehaviour // Todo: Should not be MonoBehaviour. But there should be a representaiton for the block with monobehaviour so ...
	{
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



		[NonSerialized]
		[HideInInspector]
		public BoxCollider Collider;



		[NonSerialized]
		public float sidelength; // Todo: @Math should this be moved to BlockDefinition? What is it used for? (since not all blocks will be square)

		[NonSerialized]
		public float volume; // Todo: @Math idk if buoyancy will be as simple as just this.

		#region Rendering

		[NonSerialized]
		protected MeshFilter meshFilter;

		[NonSerialized]
		protected Renderer renderer;

		[NonSerialized]
		public Material[] matArray;

		[NonSerialized]
		public Animator BlockAnim; // todo: @Math is this VehicleEditor only ?

		#endregion Rendering

		void Awake()
		{
			myBlockDefinition = BlockDefinition.Definitions[blockID];

			meshFilter = this.GetComponentInChildren<MeshFilter>();
			renderer = this.GetComponentInChildren<Renderer>();
			Collider = GetComponent<BoxCollider>();
			matArray = this.GetComponentInChildren<Renderer>().materials;
			BlockAnim = GetComponent<Animator>();
		}


		// Start is called before the first frame update
		void Start() { }

		// Update is called once per frame
		void Update() { }

		public virtual void Init() { }

		public void SetMaterial(Material mat)
		{
			renderer.material = mat;
		}

		public void SetAllMaterials(Material[] newMats)
		{
			// Material[] matArray = GetComponentInChildren<Renderer>().materials;
			//  for (int i = 0; i<matArray.Length; i++)
			//  {
			//       matArray[i] = newMats[i];
			//   }

			renderer.materials = newMats;
		}

		public void ResetMaterials()
		{
			renderer.materials = matArray;
		}

		public Material[] GetAllMaterials()
		{
			Material[] currentMats = renderer.materials;
			return currentMats;
			/*List<Material> matArray = new List<Material>();
			this.GetComponentInChildren<Renderer>().GetMaterials(matArray);
			return matArray.ToArray();
			*/
		}

		public virtual void SetGhost()
		{
			BlockAnim.SetBool("BlockGhost", true);
		}

		public virtual void SetSolid()
		{
			BlockAnim.SetBool("BlockGhost", false);
		}
	}
}