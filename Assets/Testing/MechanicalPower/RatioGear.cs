using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	/// <summary>
	/// A <see cref="ShaftComponent"/> that links two <see cref="ShaftNetwork"/>s with a fixed ratio between them.
	/// </summary>
	public class RatioGear : ShaftEdgeComponent2
	{

		public float gearRatio;



	/// <inheritdoc />
		public override List<ShaftNetwork> CurrentlyConnectedNetworks(ShaftNetworkGroup activeNetwork)
		{
			var result = new List<ShaftNetwork>();
			if (activeNetwork.Contains(network))
				result.Add(network2);
			else
				result.Add(network);
			return result;
		}

		/// <inheritdoc />
		public override ConversionInfo GetConversionFactors(ShaftNetwork @from, ShaftNetwork to)
		{
			ConversionFactorsSanityCheck(from, to);
			if (from == network)
			{
				return new ConversionInfo(gearRatio, gearRatio, gearRatio);
			}
			else
			{
				float oneOverGearRatio = 1 / gearRatio;
				return new ConversionInfo(oneOverGearRatio, oneOverGearRatio, oneOverGearRatio);
			}
		}

		/// <inheritdoc />
		public override void ShaftUpdate(ShaftNetworkGroup activeNetwork)
		{
			activeNetwork.CU.AddFriction(activeNetwork.CU.RPM * frictionLoss);
		}
	}
}
