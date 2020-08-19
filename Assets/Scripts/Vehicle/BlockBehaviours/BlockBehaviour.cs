using System;

using BlockDefinitions;

using UnityEngine;

using Vehicle.Blocks;

namespace BlockBehaviours
{
	public class BlockBehaviour : MonoBehaviour
	{
		public Block blockDesign;

		public BlockID blockID => blockDesign.blockID;
		public BlockDefinition myBlockDefinition => blockDesign.myBlockDefinition;


		#region DefinitionShortcuts

		public float Mass => myBlockDefinition.Mass;

		#endregion DefinitionShortcuts

		// todo: Only for existing code compatibility
		public GameObject gameObject;
		public float sidelength = 0.25f;


		#region Visual Representation
		// Todo: Simple blocks should be merged, both for collision and rendering.


		[NonSerialized]
		[HideInInspector]
		public BoxCollider Collider; // todo: inherited from MonoBehaviour?

		[NonSerialized]
		public MeshFilter meshFilter;

		//[NonSerialized]
		//public Renderer renderer; // inherited from MonoBehaviour

		[NonSerialized]
		public Material[] matArray;

		[NonSerialized]
		public Animator BlockAnim; // @Math is this VehicleEditor only ?
								   // @Leopard yes at the moment - but we might want to keep it around becasue we can also use it for damage/fire animations.

		#endregion Visual Representation


		void Awake()
		{
			Collider = GetComponent<BoxCollider>();
			meshFilter = this.GetComponentInChildren<MeshFilter>();
			matArray = this.GetComponentInChildren<Renderer>().materials;
			BlockAnim = GetComponent<Animator>();
		}

		#region More visual stuff
		public void SetMaterial(Material mat)
		{
			GetComponent<Renderer>().material = mat;
		}

		public void SetAllMaterials(Material[] newMats)
		{
			//Material[] matArray = GetComponentInChildren<Renderer>().materials;
			//for (int i = 0; i<matArray.Length; i++)
			//{
			//	matArray[i] = newMats[i];
			//}

			GetComponent<Renderer>().materials = newMats;
		}

		public void ResetMaterials()
		{
			GetComponent<Renderer>().materials = matArray;
		}

		public Material[] GetAllMaterials()
		{
			Material[] currentMats = GetComponent<Renderer>().materials;
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
		#endregion more visual stuff


			blockDefinition = BlockDefinition.Definitions[blockID];
		}


		#endregion Visual Representation
	}
}
