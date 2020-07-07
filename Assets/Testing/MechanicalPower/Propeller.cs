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

		//TODO: Answer from Math - Yes, possibly; though rather than physically simulate actual pressure equations for the output of the blade,
		//TODO: we can just use "set parameters" calculated elsehwere; this would mean that prop would need a float telling it the right "movement force" (how much force is added to the vehicle in the direction of the prop) above and below water.
		//TODO: likewise, the ReactionTorque below will obviously be different if it's in or out of water - but this will also need a blade-stress float; if torque exceeds blade-stress, it will break and produce NO force, only add a small amount of torque
		//TODO: back into the system, but also maybe an event? or something that tells the physical model to do a break animation etc.
        //TODO: This bladeStress can be modified by the preFab of the prop it's in, so we can set it much lower for aircraft props (which wil break in water), or even expose it to the Player for custom props (with higher-stress tolerance = larger mass)

		public float bladeStress;
		public float altitude; // Rather than trying to pass the rigidbody down via the ShaftNetworkManager to the ShaftNetworkGroup to the Network to the item; we'll just update the altitude from the prop Block object which will need linked to each Propellor script anyway.
                               // Prop Blocks will pull in location information which for large vehicles prop may  be some way away from the rigidbody's location. We can get the exact centrepoint of the prop.


		/// <inheritdoc />
		public override void ShaftUpdate()
		{
			var cu = network.networkGroup.CU;

			float rpm = cu.RPM;

			float forwardAirSpeed = 1; //TODO get airspeed at the position of the propeller (because vehicle/subgrid rotation etc).
			float pressure = 1;

            //TODO: We really should check if the altitude is actually above or below water since sealevel is 0 but waves might be higher or lower; right now we just say <0 = underwater, >0 = in the air.
            //TODO: We do have a way to check that, but it requires a lookup on teh watercontroller; this will mean a reference to the watercontroller object will be needed; might be easier to do this once on every VehicleController (which each vehicle has one of).
			if (altitude > 0)
			{
				pressure = PressureValues.AirPressure(altitude);
			}
			else
			{
				altitude = Math.Abs(altitude);
				pressure = PressureValues.WaterPressure(altitude);
			}
			//TODO: Water? -> In water more frition/pressure etc.

			float torque = forwardAirSpeed * pressure * rpm * radius * bladeCount; //TODO: equation

			float forwardForce = pressure * rpm * radius * bladeCount; //TODO: equation

			cu.AddTorque(torque);

			//TODO Add force to vehicle (subGrid)
			//rigidBody.AddForce(thisBlock.Forward * forwardForce); // or something like that <-
		}
	}
}
