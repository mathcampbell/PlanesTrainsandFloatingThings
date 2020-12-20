using System;

using UnityEngine;

using DataTypes;


namespace Vehicle
{
	/// <summary>
	/// The behaviour that manages the main vehicle's GameObject through an instance of <see cref="VehicleData"/>.
	/// </summary>
	public class VehicleBehaviour : M0noBehaviour
	{


		private VehicleData vehicleData;


		/// <inheritdoc />
		protected override void OnStart()
		{
			if (null == vehicleData) throw new Exception($"{nameof(VehicleBehaviour)} with null {nameof(VehicleData)}");

			vehicleData.Initialize();
		}



		void FixedUpdate()
		{
			vehicleData.UnityFixedUpdate();
		}

		void Update()
		{
			vehicleData.UnityUpdate();
		}


	}
}
