using System;
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
public class Clutch : ShaftEdgeComponent
{
	protected ShaftNetwork network2;

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

	public override void ShaftUpdate(ShaftNetworkGroup activeNetwork)
	{
		var active = activeNetwork.Cu; // Shortcut.

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
			var other = network.superNetwork.Cu;
			if (activeNetwork.Contains(network))
				other = network2.superNetwork.Cu; // Shortcut.

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

			// TODO: Math pulled out of hat.
			float torque = rpmDelta * torqueCapacity * application;

			active.AddTorque(torque);
		}
	}
}
}
