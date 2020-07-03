﻿using System;
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
/// Class responsible for managing the (Super)<see cref="Network"/>s of a vehicle.
/// TODO: Docked vehicles.
/// </summary>
public class NetworkManager : MonoBehaviour
{
	private GameObject myVehicle;

	private List<Network> networks;

	private List<SuperNetwork> superNetworks;


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


		foreach (var super in superNetworks)
			super.ShaftUpdate();
	}

	private void ReconfigureTopology()
	{
		foreach (var network in networks)
			network.needsReconfiguration = false;

		throw new NotImplementedException();
	}
}
}
