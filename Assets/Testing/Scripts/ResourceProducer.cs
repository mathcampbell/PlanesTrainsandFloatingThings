using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceProducer : ResourceNetworkItem
{
	public readonly float maxProduction = 1;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{

	}

	/// <summary>
	/// Find the amount of resource that could be produced this tick
	/// </summary>
	/// <returns></returns>
	public abstract float PotentialProduction();

	/// <summary>
	/// Produce the given amount of resource. (This method is to trigger any side effacts the producion may have)
	/// </summary>
	/// <param name="fractionOfPotential"></param>
	public abstract void Produce(float fractionOfPotential);
}
