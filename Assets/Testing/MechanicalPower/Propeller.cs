using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	public class Propeller : ShaftComponent
	{
		/// <summary>
		/// The radius of the propeller.
		/// </summary>
		private float radius;

		/// <summary>
		/// The amount of blades on the propeller
		/// </summary>
		private int bladeCount;

		//TODO: Other performance parameters of the propeller.
		//TODO? ideally water and air propellers just vary in these parameters and we don't have seperate classes. Is this feasable?





		/// <inheritdoc />
		public override void ShaftUpdate()
		{
			var cu = network.superNetwork.CU;

			float rpm = cu.RPM;

			float forwardAirSpeed = 1; //TODO get airspeed at the position of the propeller (because vehicle/subgrid rotation etc).
			float pressure = 1;       //TODO compute from altitude (vehicle's altitude will be close enough for the math).

			//TODO: Water? -> In water more frition/pressure etc.

			float torque = forwardAirSpeed * pressure * rpm * radius * bladeCount; //TODO: Math

			float forwardForce = pressure * rpm * radius * bladeCount; //TODO: Math

			cu.AddTorque(torque);

			//TODO Add force to vehicle (subGrid)
			//rigidBody.AddForce(thisBlock.Forward * forwardForce); // or something like that <-
		}
	}
}
