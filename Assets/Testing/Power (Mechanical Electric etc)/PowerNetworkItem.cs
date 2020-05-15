using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item in a power network
/// </summary>
public abstract class PowerNetworkItem : MonoBehaviour
{
	/// <summary>
	/// The network manager this item is part of.
	/// </summary>
	PowerNetworkManager manager;

	/// <summary>
	/// The power type.
	/// </summary>
	PowerType powerType => manager.powerType;


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		
	}
}
