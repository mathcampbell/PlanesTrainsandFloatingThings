using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Testing.MechanicalPower
{
	public class CombustionEngine : ShaftComponent
	{
		/// <summary>
		/// Throttle, [-1..1]
		/// </summary>
		public float throttle;

		/// <summary>
		/// The torque/RPM curve of the engine. Must be manually set in Editor and saved to Engine prefab.
		/// </summary>
		public AnimationCurve torqueCurve; // This would normally run from 0 to 7200 RPM for most gasoline engines; torque produced will change for each engine.


		/// <summary>
		/// The RPM at which torque drops to zero because of the redline.
		/// </summary>
		public float maxRPM; // 7200


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

			if(absRPM < maxRPM)
			{
				torque = throttle * torqueCurve.Evaluate(absRPM);
			}
			else
			{
				// else torque = 0
			}

			cu.AddTorque(torque);

			// https://www.quora.com/What-is-the-formula-to-calculate-the-power-consumed-by-an-electric-motor
			float powerDrawWatt = 2 * Mathf.PI * absRPM * torque / 60;
		}
	}
}
