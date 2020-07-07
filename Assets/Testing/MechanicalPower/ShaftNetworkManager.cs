using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Testing.MechanicalPower
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
			ReconfigureTopology();
		}

		throw new NotImplementedException();
	}
}
}
