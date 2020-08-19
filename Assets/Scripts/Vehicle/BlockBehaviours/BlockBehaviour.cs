using System;

using BlockDefinitions;

using UnityEngine;

using Vehicle.Blocks;

namespace BlockBehaviours
{
	public class BlockBehaviour : MonoBehaviour
	{
		public Block blockDesign;

		[NonSerialized]
		public BlockDefinition blockDefinition;

		public BlockID blockID;

		#region Visual Representation
		// Todo: Simple blocks should be merged, both for collision and rendering.

		[NonSerialized]
		public MeshFilter meshFilter;

		[NonSerialized]
		public Renderer renderer;


		void Awake()
		{
			blockDefinition = BlockDefinition.Definitions[blockID];
		}


		#endregion Visual Representation
	}
}
