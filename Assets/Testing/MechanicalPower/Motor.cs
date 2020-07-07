using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Testing.MechanicalPower
{
	public class Motor : ShaftComponent
	{
		/// <summary>
		/// Throttle, [-1..1]
		/// </summary>
		public float throttle;

		//TODO: Convert to torque curve.
		/// <summary>
		/// The RPM at which torque starts to decrease.
		/// </summary>
		public float peakTorqueRPM; // 50

		/// <summary>
		/// The RPM at which torque output becomes zero.
		/// </summary>
		public float maxRPM; // 100


		/// <summary>
		/// The maximum output torque. (at 100% <see cref="throttle"/>
		/// </summary>
		public float torqueOutput;


		public override void ShaftUpdate()
		{
<<<<<<< HEAD
			var cu = network.superNetwork.Cu;
=======
			var cu = network.networkGroup.CU;
>>>>>>> 61c77ff81fb13677a623385ef937fd015461042c

			cu.AddFriction(cu.RPM * frictionLoss);

			float absRPM = Math.Abs(cu.RPM);

			float torque = 0;

			if(absRPM < peakTorqueRPM)
			{
				torque = throttle * peakTorqueRPM;
			} else
			{
				if(absRPM < maxRPM)
				{ // peakTorqueRPM < absRPM < maxRPM
					float multiplier = 1 - ((absRPM - peakTorqueRPM) / (maxRPM - peakTorqueRPM));

					multiplier = Math.Max(1, multiplier);

					torque = throttle * torqueOutput * multiplier;
				}
				// else torque = 0
			}

			cu.AddTorque(torque);

			// https://www.quora.com/What-is-the-formula-to-calculate-the-power-consumed-by-an-electric-motor
			float powerDrawWatt = 2 * Mathf.PI * absRPM * torque / 60;
			//TODO: Draw the electrical power from somewhere.
			// Note draw in WATT (units / second), adjust consumption from battery with Time.fixedDeltaTime
		}
	}
}
