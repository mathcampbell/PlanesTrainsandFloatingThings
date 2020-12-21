using System;
using System.Collections.Generic;

using UnityEngine;

namespace Vehicle.MechanicalPower
{
	/// <summary>
	/// A <see cref="ShaftNetwork"/> is a collection of directly connected <see cref="ShaftComponent"/>s.
	/// </summary>
	public class ShaftNetwork
	{
		/// <summary>
		/// The <see cref="ShaftNetworkGroup"/> that this <see cref="ShaftNetwork"/> is part of.
		/// </summary>
		public ShaftNetworkGroup networkGroup;


		public ShaftNetworkManager manager => networkGroup.manager;


		/// <summary>
		/// All <see cref="ShaftComponent"/>s in this network.
		/// </summary>
		public List<ShaftComponent> components;

		/// <summary>
		/// The <see cref="ShaftComponent"/>s in this network that are not <see cref="ShaftEdgeComponent"/>
		/// </summary>
		public List<ShaftComponent> nonEdgeComponents;

		/// <summary>
		/// The <see cref="ShaftComponent"/>s in this network that are also <see cref="ShaftEdgeComponent"/>
		/// </summary>
		public List<ShaftEdgeComponent> edgeComponents;


		/// <summary>
		/// <see cref="ShaftNetwork"/>s that we could form a <see cref="ShaftNetworkGroup"/> with.
		/// </summary>
		public List<ShaftNetwork> potentialNeighbours;

		public List<ShaftNetwork> currentNeighbours;


		/// <summary>
		/// The inertia of this <see cref="ShaftNetwork"/>.
		/// </summary>
		public float inertia { get; set; }


		internal bool needsReconfiguration = true;

		/// <summary>
		/// Signal to this network that it needs to reconfigure the <see cref="ShaftNetworkGroup"/> topography.
		/// (This will NOT happen immediately)
		/// </summary>
		public void SignalNeedReconfiguration()
		{
			needsReconfiguration = true;
			manager.NeedsReconfiguration = true;
		}

	}
}