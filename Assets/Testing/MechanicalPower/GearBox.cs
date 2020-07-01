using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	/// <summary>
	/// A <see cref="Component"/> that links two <see cref="Network"/>s with a selectable ratio between them
	/// </summary>
	public class GearBox : EdgeComponent
	{
		protected Network network2;

		int selectedRatioIndex;
		List<float> ratios;




		public override List<Network> CurrentleConnectedNetworks(SuperNetwork activeNetwork)
		{
			throw new NotImplementedException();
		}

		public override void ShaftUpdate(SuperNetwork activeNetwork)
		{
			throw new NotImplementedException();
		}
	}
}
