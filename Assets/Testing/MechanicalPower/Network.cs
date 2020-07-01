using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Testing.MechanicalPower
{
/// <summary>
/// A <see cref="Network"/> is a collection of directly connected <see cref="Component"/>s.
/// </summary>
public class Network : MonoBehaviour
{
	/// <summary>
	/// The <see cref="SuperNetwork"/> that this <see cref="Network"/> is part of.
	/// </summary>
	public SuperNetwork superNetwork;



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


	/// <summary>
	/// The inertia of this <see cref="Network"/>.
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
}