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


		#region RuntimeState, Todo: (re)move


		[NonSerialized]
		[HideInInspector]
		public BoxCollider Collider;



		[NonSerialized]
		public float sidelength; // Todo: @Math should this be moved to BlockDefinition? What is it used for? (since not all blocks will be square)
								 //		@Leopard: this was a poor attempt at a simplified centre of gravity & mass solution that isn't quite implemented yet; since we can't get the CoG using Unity's physics cos we aren't making every block a Rigidbody
								 //				we will need to manually calculate it, which therefore required knowing how big each block it, it's relative position in the vehicle and it's mass, to accurate get the CoG and CoM.
		[NonSerialized]
		public float volume; // Todo: @Math idk if buoyancy will be as simple as just this. If it's to be used: should be moved to Definition. @Leopard - yeah this was before we worked out about enclosed volumes etc;
							//			we might still want to use it though - if we have the mass & volume of each block, we can get density, which is needed for the bouyancy. We'd just have to remember to add on the enclosed volumes too (at their relative mass/density, which might change based on water or gas content).
        #region Rendering

        [NonSerialized]
		protected MeshFilter meshFilter;

		[NonSerialized]
		protected Renderer renderer;

		[NonSerialized]
		public Material[] matArray;

		[NonSerialized]
		public Animator BlockAnim; // todo: @Math is this VehicleEditor only ? @Leopard yes at the moment - but we might want to keep it around becasue we can also use it for damage/fire animations.

		#endregion RuntimeState, Todo: (re)move

		#endregion Rendering

		void Awake()
		{
			myBlockDefinition = BlockDefinition.Definitions[blockID];

			// todo: runtime stuff: (re)move
			meshFilter = this.GetComponentInChildren<MeshFilter>();
			renderer = this.GetComponentInChildren<Renderer>();
			Collider = GetComponent<BoxCollider>();
			matArray = this.GetComponentInChildren<Renderer>().materials;
			BlockAnim = GetComponent<Animator>();
		}


		// Start is called before the first frame update
		void Start() { } // todo: MonoBehaviour stuff: remove

		// Update is called once per frame
		void Update() { } // todo: MonoBehaviour stuff: remove

		public virtual void Init() { }

		public void SetMaterial(Material mat)
		{ // todo: runtime stuff: (re)move
			renderer.material = mat;
		}

		public void SetAllMaterials(Material[] newMats)
		{ // todo: runtime stuff: (re)move
			//Material[] matArray = GetComponentInChildren<Renderer>().materials;
			//for (int i = 0; i<matArray.Length; i++)
			//{
			//	matArray[i] = newMats[i];
			//}

			renderer.materials = newMats;
		}

		public void ResetMaterials()
		{ // todo: runtime stuff: (re)move
			renderer.materials = matArray;
		}

		public Material[] GetAllMaterials()
		{ // todo: runtime stuff: (re)move
			Material[] currentMats = renderer.materials;
			return currentMats;
			/*List<Material> matArray = new List<Material>();
			this.GetComponentInChildren<Renderer>().GetMaterials(matArray);
			return matArray.ToArray();
			*/
		}

		public virtual void SetGhost()
		{ // todo: runtime stuff: (re)move
			BlockAnim.SetBool("BlockGhost", true);
		}

		public virtual void SetSolid()
		{ // todo: runtime stuff: (re)move
			BlockAnim.SetBool("BlockGhost", false);
		}
	}
}