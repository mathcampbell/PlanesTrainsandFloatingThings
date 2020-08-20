using System;
using System.IO;

using BlockDefinitions;

using UnityEngine;

using Vehicle.Blocks;

namespace Vehicle.BlockBehaviours
{
	/// <summary>
	/// The Runtime-Simulation state of a block
	/// </summary>
	public class BlockBehaviour : MonoBehaviour, ISerializationCallbackReceiver
	{
		/// <summary>
		/// Used to serialize things Unity can't.
		/// </summary>
		[SerializeField]
		private byte[] unitySerializationData;


		[NonSerialized] // Unity can't serialize this.
		public Block blockDesign;

		public BlockID blockID => blockDesign.blockID;
		public BlockDefinition myBlockDefinition => blockDesign.myBlockDefinition;


		#region DefinitionShortcuts

		public float Mass => myBlockDefinition.Mass;

		#endregion DefinitionShortcuts

		// todo: Only for existing code compatibility
		[NonSerialized]
		public GameObject gameObject;
		public float sidelength => myBlockDefinition.sidelength;


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



		/// <inheritdoc />
		public void OnBeforeSerialize()
		{
			// Unity: before serialization -> serialize fields Unity can't to byteArray.
			using (var stream = new MemoryStream())
			{
				Block.WriteToStreamBinary(stream, blockDesign);

				// other fields

				unitySerializationData = stream.ToArray();
			}
		}

		/// <inheritdoc />
		public void OnAfterDeserialize()
		{
			// Unity: after deserialization -> deserialize fields Unity can't from byteArray.
			if (null == unitySerializationData)
			{
				Debug.LogError($"{nameof(unitySerializationData)} was null: no data was loaded.");
				return;
			}
			using (var stream = new MemoryStream(unitySerializationData))
			{
				unitySerializationData = null; // The data has bean read into the stream, free up the space.

				blockDesign = Block.ReadFromStreamBinary(stream);

				// other fields
			}
		}
	}
}
