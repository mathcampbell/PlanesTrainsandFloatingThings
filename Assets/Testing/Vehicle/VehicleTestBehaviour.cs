using DataTypes;

using UnityEngine;

using Vehicle;

namespace Testing.Vehicle
{
	public class VehicleTestBehaviour : M0noBehaviour
	{
		private GameObject vehicleObject;


		/// <inheritdoc />
		protected override void OnEnableAndAfterStart()
		{
			if (null != vehicleObject)
			{
				Destroy(vehicleObject);
			}

			vehicleObject = new GameObject("vehicle");

			var vehicleBehaviour = vehicleObject.AddComponent<VehicleBehaviour>();

			vehicleBehaviour.vehicleData = ZHardcodeVehicleDesigns.vehicle1;
		}
	}
}
