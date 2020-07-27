using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BlockDefinitions;

namespace Testing.Serialization
{

	public class BlockDefinitionBehaviour : MonoBehaviour
	{
		public BlockDefinition myDefinition;


		// Start is called before the first frame update
		void Start()
		{
			BlockDefinition.LoadAllDefinitions();
		}

		// Update is called once per frame
		void Update() { }
	}
}