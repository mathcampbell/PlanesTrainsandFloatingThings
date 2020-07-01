using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item in a resource network
/// </summary>
public abstract class ResourceNetworkItem : MonoBehaviour
{
	/// <summary>
	/// The network manager this item is part of.
	/// </summary>
	ResourceNetworkManager manager;

	/// <summary>
	/// The resource
	/// </summary>
	Resource resource => manager.resource;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{

	}
}
