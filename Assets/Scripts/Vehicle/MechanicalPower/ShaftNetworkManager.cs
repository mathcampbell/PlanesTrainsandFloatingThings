using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Vehicle.MechanicalPower
{
	/// <summary>
	/// Class responsible for managing the (Super)<see cref="ShaftNetwork"/>s of a vehicle.
	/// TODO: Docked vehicles.
	/// </summary>
	public class ShaftNetworkManager
	{
		private VehicleData parent;

		private List<ShaftNetwork> networks = new List<ShaftNetwork>();

		private List<ShaftNetworkGroup> networkGroups = new List<ShaftNetworkGroup>();


		internal bool NeedsReconfiguration = true;
		private void FixedUpdate()
		{
			// todo: multiThreading, this is 99% pure C# so we can run it in parallel,
			// and only synchronize here instead of running all the computations
			if (NeedsReconfiguration)
			{
				ReconfigureTopology();
			}


			foreach (var networkGroup in networkGroups)
				networkGroup.ShaftUpdate();
		}

		public void Initialize()
		{
			CreateNetworks();
			ReconfigureTopology();
		}

		private void CreateNetworks()
		{
			var temp = parent.blocks.Where(b => b.IsShaft || b.IsShaftComponent).ToArray();

			var allShafts = temp.Where(b => b.IsShaft);
			var allShaftComponentBlocks = temp.Where(b => b.IsShaftComponent);


		}

		private void ReconfigureTopology()
		{
			var toReconfigure = new HashSet<ShaftNetworkGroup>();

			// Discover networks that need to be reconfigured.
			foreach (var network in networks)
			{
				if (network.needsReconfiguration)
				{
					toReconfigure.Add(network.networkGroup);
					network.needsReconfiguration = false;
				}
			}

			foreach (var networkGroup in toReconfigure)
			{
				networkGroup.ReconfigureTopology();
			}

			NeedsReconfiguration = false;

			// todo: I guess that something more needs to happen at this point, but I can't remember what that would be.
			// Let's see if it breaks or not.
			//throw new NotImplementedException();
		}
	}
}
