using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
	/// <summary>
	/// Contains data for the conversion between <see cref="ShaftNetwork"/>s
	/// </summary>
	public readonly struct ConversionInfo
	{
		/// <summary>
		/// RPM conversion factor
		/// </summary>
		public readonly float RPM;

		/// <summary>
		/// Torque conversion factor
		/// </summary>
		public readonly float Torque;

		/// <summary>
		/// Orientation conversion factor
		/// </summary>
		public readonly float Orientation;

		public ConversionInfo(float rPM, float torque, float orientation)
		{
			RPM         = rPM;
			Torque      = torque;
			Orientation = orientation;
		}

		public void Deconstruct(out float RPM, out float Torque, out float Orientation)
		{
			RPM = this.RPM;
			Torque = this.Torque;
			Orientation = this.Orientation;
		}
	}
}
