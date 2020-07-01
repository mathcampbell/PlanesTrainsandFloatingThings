using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Network : MonoBehaviour
{
	/// <summary>
	/// All <see cref="Component"/>s in this network.
	/// </summary>
	public List<Component> components;

	/// <summary>
	/// The <see cref="Component"/>s in this network that are not <see cref="EdgeComponent"/>
	/// </summary>
	public List<Component> nonEdgeComponents;

	/// <summary>
	/// The <see cref="Component"/>s in this network that are also <see cref="EdgeComponent"/>
	/// </summary>
	public List<EdgeComponent> edgeComponents;

	/// <summary>
	/// <see cref="Network"/>s that we could form a <see cref="SuperNetwork"/> with.
	/// </summary>
	public List<Network> potentialNeighbours;

	public SuperNetwork superNetwork;

	/// <summary>
	///
	/// </summary>
	public float inertia { get; set; }


	bool needsReconfiguration = false;

	/// <summary>
	/// Signal to this network that it needs to reconfigure the <see cref="SuperNetwork"/> topography.
	/// (This will NOT happen immediately)
	/// </summary>
	public void SignalNeedReconfiguration()
	{
		needsReconfiguration = true;
		// TODO: The effect still needs to be implemented.
		throw new NotImplementedException();
	}

}