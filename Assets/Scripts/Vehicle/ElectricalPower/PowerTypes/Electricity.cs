namespace Vehicle.ElectricalPower.PowerTypes {
	public class Electricity : PowerType
	{
		public Electricity()
		{
			name = "Electricity";
			storageUnit = "Joule";
			usageUnit = "Watt";

			//useTorque = false;
		}
	}
}
