using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

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

		#region Operators

		public static ConversionInfo operator -(ConversionInfo a)
		{
			(float rpm, float torque, float orientation) = a;
			return new ConversionInfo(1/rpm, 1/torque, 1/orientation);
		}

		public static ConversionInfo operator *(ConversionInfo left, ConversionInfo right)
		{
			(float r1, float t1, float o1) = left;
			(float r2, float t2, float o2) = right;
			return new ConversionInfo(rPM: r1 * r2, torque: t1 * t2, orientation: o1 * o2);
		}

		public static ConversionInfo operator *(ConversionInfo left, float right)
		{
			(float rpm, float torque, float orientation) = left;
			return new ConversionInfo(rPM: rpm * right, torque: torque * right, orientation: orientation * right);
		}

		public static ConversionInfo operator /(ConversionInfo left, float right)
		{
			(float rpm, float torque, float orientation) = left;
			return new ConversionInfo(rPM: rpm / right, torque: torque / right, orientation: orientation / right);
		}



		#endregion Operators
	}
}
