using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item in a power network
/// </summary>
public abstract class MechanicalNetworkItem : MonoBehaviour
{
	/// <summary>
	/// The network manager this item is part of.
	/// </summary>
	public MechanicalNetworkManager manager;

	public List<MechanicalNetworkItem> DirectlyConnected;

	/// <summary>
	/// The power type.
	/// </summary>
	MechanicalPowerType powerType => manager.powerType;


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		
	}

	public abstract void AddToNetwork();

	public abstract void RemoveFromNetwork();
    
}
