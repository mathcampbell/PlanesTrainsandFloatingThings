using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceStorage : ResourceNetworkItem
{
	public float available;
	public readonly float capacity;


	public float capacityFree => capacity - available;

	//public float maxOutput = 1;
	//public float maxinput = 1;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{

	}
}
