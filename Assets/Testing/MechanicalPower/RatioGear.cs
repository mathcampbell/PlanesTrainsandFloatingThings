using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	public class RatioGear : EdgeComponent
	{
		protected Network network2;

		public float gearRatio;

		/// <summary>
		/// Friction losses of the component. (Torque per RPM)
		/// </summary>
		public float frictionLoss;



		public override List<Network> CurrentleConnectedNetworks(SuperNetwork activeNetwork)
		{
			var result = new List<Network>();
			if (activeNetwork.Contains(network))
				result.Add(network2);
			else
				result.Add(network);
			return result;
		}

		public override void ShaftUpdate(SuperNetwork activeNetwork)
		{
			activeNetwork.CU.AddFriction(activeNetwork.CU.RPM * frictionLoss);
		}
	}
}
