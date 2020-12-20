using System;

using UnityEngine;

using DataTypes;


namespace Vehicle
{
	/// <summary>
	/// The behaviour that manages the main vehicle's GameObject through an instance of <see cref="VehicleData"/>.
	/// </summary>
	[DefaultExecutionOrder(defaultExecutionOrder)]
	public class VehicleBehaviour : M0noBehaviour
	{
		public const int defaultExecutionOrder = 0;

		[NonSerialized] // todo: we probably should in some way to make hot-reload work, but Unity is too dumb to do it properly.
		public VehicleData vehicleData;


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
