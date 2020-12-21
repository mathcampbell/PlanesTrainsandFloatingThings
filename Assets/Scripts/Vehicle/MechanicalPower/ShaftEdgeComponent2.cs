using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle.MechanicalPower
{
	/// <summary>
	/// A <see cref="ShaftEdgeComponent"/> with exactly two networks.
	/// </summary>
	public abstract class ShaftEdgeComponent2 : ShaftEdgeComponent
	{
		public ShaftNetwork network2;

		protected void ConversionFactorsSanityCheck(ShaftNetwork from, ShaftNetwork to)
		{
			if (from == to) throw new ArgumentException("Both networks are the same network.");

			bool fromWrong = !(from == network || from == network2);
			bool toWrong = !(to     == network || to   == network2);
			if (fromWrong || toWrong)
			{ // Specified networks that we don't have. TODO? Is this overkill?
				string text = "Invalid argument";
				if (fromWrong && toWrong) text += "s";
				text += " ";
				if (fromWrong) text += nameof(from) + " ";
				if (toWrong)   text += nameof(to);
				text += " are not networks that this " + nameof(ShaftEdgeComponent2) + " is directly connected to.";

				throw new ArgumentException(text);
			}
		}
	}
}
