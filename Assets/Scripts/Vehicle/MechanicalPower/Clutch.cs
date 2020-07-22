﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
/// <summary>
/// A <see cref="ShaftComponent"/> that can connect or disconnect two <see cref="ShaftNetwork"/>s.
/// </summary>
public class Clutch : ShaftEdgeComponent2
{
	/// <summary>
	/// 0 disconnected, 1 fully connected.
	/// </summary>
	float application;

	/// <summary>
	/// Maximum torque before slipping occurs (at 100% <see cref="application"/>).
	/// </summary>
	float torqueCapacity;

	/// <summary>
	/// Are the networks on both sides rotating at the same speed, allowing a <see cref="ShaftNetworkGroup"/> to span both sides?
	/// </summary>
	bool isEqualized;

	/// <summary>
	/// RPM delta Threshold below which we consider both sides rotating at the same speed.
	/// </summary>
	const float rpmDeltaThreshold = 0.001f;


	/// <inheritdoc />
	public override List<ShaftNetwork> CurrentlyConnectedNetworks(ShaftNetworkGroup activeNetwork)
	{
		var result = new List<ShaftNetwork>();
		if (isEqualized)
		{
			if (activeNetwork.Contains(network))
				result.Add(network2);
			else
				result.Add(network);
		}
		return result;
	}

	/// <inheritdoc />
	public override ConversionInfo GetConversionFactors(ShaftNetwork @from, ShaftNetwork to)
	{
		ConversionFactorsSanityCheck(from, to);
		if (isEqualized)
		{
			return new ConversionInfo(1, 1, 1);
		}
		else
		{
			throw new InvalidOperationException("Clutch is not equalized, no conversion factors exist at this time.");
		}
	}

	/// <inheritdoc />
	public override void ShaftUpdate(ShaftNetworkGroup activeNetwork)
	{
		var active = activeNetwork.CU; // Shortcut.

		active.AddFriction(frictionLoss * active.RPM);

		if(application < 1)
		{
			if (isEqualized)
			{
				isEqualized = false;
				network.SignalNeedReconfiguration();
				network2.SignalNeedReconfiguration();
			}
		}

		if (false == isEqualized)
		{
			var other = network.networkGroup.CU;
			if (activeNetwork.Contains(network))
				other = network2.networkGroup.CU; // Shortcut.

			float rpmDelta = active.RPM - other.RPM;
			// TODO? Allow merger at application < 1 ?
			// Currently, once merged, we will never start slipping again.
			// So that would need to be handled.
			if (application >= 1 && Math.Abs(rpmDelta) < rpmDeltaThreshold)
			{
				isEqualized = true;
				network.SignalNeedReconfiguration();
				network2.SignalNeedReconfiguration();
				// Reconfiguration will NOT happen immediately.
			}

			// TODO: equation pulled out of hat.
			float torque = rpmDelta * torqueCapacity * application;

			active.AddTorque(torque);
		}
	}
}
}