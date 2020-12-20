using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Vehicle.MechanicalPower
{

/* Shaft stuff general list of things to do
 * TODO: Friction calculation: Check the math.
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */



	/// <summary>
	/// Class responsible for managing the (Super)<see cref="ShaftNetwork"/>s of a vehicle.
	/// TODO: Docked vehicles.
	/// </summary>
	public class ShaftNetworkManager : MonoBehaviour
	{
		private GameObject myVehicle;

		private List<ShaftNetwork> networks;

		private List<ShaftNetworkGroup> networkGroups;



		private void FixedUpdate()
		{
			// todo: multiThreading, this is 99% pure C# so we can run it in parallel,
			// and only synchronize here instead of running all the computations
			foreach (var network in networks)
			{
				if (network.needsReconfiguration)
				{
					ReconfigureTopology();
					break;
				}
			}


			foreach (var networkGroup in networkGroups)
				networkGroup.ShaftUpdate();
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

			// todo: I guess that something more needs to happen at this point, but I can't remember what that would be.
			// Let's see if it breaks or not.
			//throw new NotImplementedException();
		}
	}
}
