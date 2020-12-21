namespace Vehicle.Volumes
{

	/// <summary>
	/// Fluid in a volume: a fluid kind and amount.
	/// </summary>
	public struct VolumeFluid
	{
		/// <summary>
		/// The fluid kind
		/// </summary>
		public Fluid Fluid;

		/// <summary>
		/// The amount of fluid measured in mass (kg)
		/// </summary>
		// We measure mass because volume could change with pressure and temperature.
		public float amount;
	}
}
