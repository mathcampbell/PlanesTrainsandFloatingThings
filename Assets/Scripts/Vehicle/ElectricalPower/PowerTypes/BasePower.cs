namespace Vehicle.ElectricalPower.PowerTypes {
	/// <summary>
	/// The base class of all power-forms.
	/// </summary>
	//This continas shared functinality
	public abstract class PowerType
	{
		/// <summary>
		/// The name of the resource
		/// </summary>
		public string name;

		/// <summary>
		/// The name of the unit used to measure stored power.
		/// </summary>
		public string storageUnit;

		/// <summary>
		/// The name of the unit used to measure usage of power.
		/// </summary>
		public string usageUnit;

	}
}
