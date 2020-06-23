using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MechanicalModifier : MechanicalNetworkItem
{
	public readonly float maxConsumption = 1;
	public float requested;
	public float recieved;
	public float torqueModifier;
    public float rpmModifier;

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
		manager.modifiers.Add(this);
	}

	public override void RemoveFromNetwork()
	{
		manager.modifiers.Remove(this);
	}


}
