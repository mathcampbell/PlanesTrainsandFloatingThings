using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	/// <summary>
	/// A <see cref="ShaftComponent"/> that represents a diffirential.
	/// </summary>
	public class Diffirential : ShaftEdgeComponent
	{
		ShaftNetwork outputLeft;
		ShaftNetwork outputRight;

		// Diffirentials commonly also have a fixed gear ratio between input and output side.
		float gearRatio;



		/// <inheritdoc />
		public override List<ShaftNetwork> CurrentlyConnectedNetworks(ShaftNetworkGroup activeNetwork)
		{
			var result = new List<ShaftNetwork>();
			if (!activeNetwork.Contains(network)) result.Add(network);
			if (!activeNetwork.Contains(outputLeft)) result.Add(outputLeft);
			if (!activeNetwork.Contains(outputRight)) result.Add(outputRight);

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

		protected void ConversionFactorsSanityCheck(ShaftNetwork @from, ShaftNetwork to)
		{
			if (@from == to) throw new ArgumentException("Both networks are the same network.");

			bool fromWrong = !(from == network || from == outputLeft || from == outputRight);
			bool toWrong   = !(to   == network || to   == outputLeft || to   == outputRight);
			if (fromWrong || toWrong)
			{ // Specified networks that we don't have. TODO? Is this overkill?
				string text = "Invalid argument";
				if (fromWrong && toWrong) text += "s";
				text += " ";
				if (fromWrong) text += nameof(from) + " ";
				if (toWrong) text += nameof(to);
				text += " are not networks that this " + nameof(ShaftEdgeComponent2) + " is directly connected to.";

				throw new ArgumentException(text);
			}
		}

		/// <inheritdoc />
		public override void ShaftUpdate(ShaftNetworkGroup activeNetwork)
		{
			throw new NotImplementedException();
		}
	}
}
