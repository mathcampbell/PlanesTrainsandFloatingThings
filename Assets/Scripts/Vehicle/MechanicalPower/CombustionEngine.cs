using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Vehicle.MechanicalPower
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

		/// <summary>
		/// The current RPM of the engine, evaluated on ShaftUpdate().
		/// </summary>
		public float currentRPM = 0;

		/// <summary>
		/// Is the Engine running? Set by Engine block componnent.
		/// </summary>
		public bool engineRunning;



		public override void ShaftUpdate()
		{
			var cu = network.networkGroup.CU;

			cu.AddFriction(cu.RPM * frictionLoss);

			currentRPM = Math.Abs(cu.RPM);

			float torque = 0;

			if(currentRPM < maxRPM && engineRunning)
			{
				torque = throttle * torqueCurve.Evaluate(currentRPM);
			}
			else
			{
				// else torque = 0
			}

			cu.AddTorque(torque);

			
		}
	}
}
