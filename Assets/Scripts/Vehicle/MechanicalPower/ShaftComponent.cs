using UnityEngine;


namespace Vehicle.MechanicalPower
{
	/// <summary>
	/// A <see cref="ShaftComponent"/> represents any object in a Shaft network that has functionality.
	/// </summary>
	public abstract class ShaftComponent : MonoBehaviour
	{
		/// <summary>
		/// The network this <see cref="ShaftComponent"/> is a part of.
		/// </summary>
		protected ShaftNetwork network;


		/// <summary>
		/// Friction losses of the component. (Torque per RPM)
		/// </summary>
		public float frictionLoss;


		/// <summary>
		/// Do shaft related actions.
		/// </summary>
		public abstract void ShaftUpdate();
	}
}