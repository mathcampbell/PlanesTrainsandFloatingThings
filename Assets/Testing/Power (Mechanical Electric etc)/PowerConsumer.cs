using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerConsumer : PowerNetworkItem
{
	public readonly float maxConsumption = 1;


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		
	}

	public override void AddToNetwork()
	{
		manager.consumers.Add(this);
	}

	public override void RemoveFromNetwork()
	{
		manager.consumers.Remove(this);
	}


	/// <summary>
	/// Find the amount of power that could be consumed this tick
	/// </summary>
	/// <returns></returns>
	public abstract float PotentialConsumption();

	/// <summary>
	/// Consume the given amount of power. (This method is to trigger any side effacts the consumption may have)
	/// </summary>
	/// <param name="fractionOfPotential"></param>
	public abstract void Consume(float fractionOfPotential);
}
