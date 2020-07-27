using System.Collections;
using System.Collections.Generic;

using Assets.Scripts.Vehicle.Blocks;

using UnityEngine;

public class BlockDefinitionBehaviour : MonoBehaviour
{
	public BlockDefinition myDefinition;


	// Start is called before the first frame update
	void Start()
	{
		BlockDefinition.LoadAllDefinitions();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
