using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Vehicle.MechanicalPower
{
	public class Wheel : ShaftComponent
	{
		private WheelCollider wheelCollider;

		/// <summary>
		/// Variable that defines how much torque is generated from the RPM difference between the wheel and the shaft.
		/// </summary>
		private float magicVariable = 1;

		/// <inheritdoc />
		public override void ShaftUpdate()
		{
			var active = network.networkGroup.CU; // Shortcut.

			// todo: Unity will complain if we are not on the main thread, probably best to put the wheelCollider in a wrapper
			// that caches the values, so we can run this code threaded.
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
