using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	/// <summary>
	/// A <see cref="Component"/> that represents a diffirential.
	/// </summary>
	public class Diffirential : EdgeComponent
	{
		Network outputLeft;
		Network outputRight;

		// Diffirentials commonly also have a fixed gear ratio between input and output side.
		float gearRatio;



		public override List<Network> CurrentlyConnectedNetworks(SuperNetwork activeNetwork)
		{
			throw new NotImplementedException();
		}

		public override void ShaftUpdate(SuperNetwork activeNetwork)
		{
			throw new NotImplementedException();
		}
	}
}
