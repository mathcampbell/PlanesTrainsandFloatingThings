using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle.Blocks.Active
{
	public class LithiumBattery : ActiveBlock
	{

		public NumericOutput CurrentCharge;


		// Start is called before the first frame update
		void Start() { }

		// Update is called once per frame
		void Update() { }

		private void FixedUpdate()
		{
			CurrentCharge.ouputNumeric = (GetComponentInChildren<BatteryPower>().available
			                            / GetComponentInChildren<BatteryPower>().capacity);
			// Returns the current charge left on a scale of 1-0;
		}
	}
}