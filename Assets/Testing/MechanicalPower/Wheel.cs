using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Testing.MechanicalPower
{
public class Wheel : Component
{
	private WheelCollider collider;

	/// <summary>
	/// Variable that defines how much torque is generated from the RPM diffirence between the wheel and the shaft.
	/// </summary>
	private float magicVariable = 1;

	/// <inheritdoc />
	public override void ShaftUpdate()
	{
		var active = network.superNetwork.CU; // Shortcut.

		float rpmDelta = collider.rpm - active.RPM;

		float torque = rpmDelta * magicVariable;
		// Options to replace magicVariable:
		// PID
		// More math? (using wheel inertia)


		//TODO: Directions of the torque could be wrong.
		active.AddTorque(torque);
		collider.motorTorque = torque;
	}
}
}
