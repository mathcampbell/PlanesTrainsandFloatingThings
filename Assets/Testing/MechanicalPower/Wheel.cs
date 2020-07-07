using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Testing.MechanicalPower
{
public class Wheel : ShaftComponent
{
	private WheelCollider wheelCollider;

	/// <summary>
	/// Variable that defines how much torque is generated from the RPM diffirence between the wheel and the shaft.
	/// </summary>
	private float magicVariable = 1;

	/// <inheritdoc />
	public override void ShaftUpdate()
	{
		var active = network.networkGroup.CU; // Shortcut.

		float rpmDelta = wheelCollider.rpm - active.RPM;

		float torque = rpmDelta * magicVariable;
		// Options to replace magicVariable:
		// PID
		// More math? (using wheel inertia)


		//TODO: Directions of the torque could be wrong.
		active.AddTorque(torque);
		wheelCollider.motorTorque = torque;
	}
}
}
