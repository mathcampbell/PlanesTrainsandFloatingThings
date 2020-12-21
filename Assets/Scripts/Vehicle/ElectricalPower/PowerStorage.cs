namespace Vehicle.ElectricalPower {
	public abstract class PowerStorage : PowerNetworkItem
	{
		public float available;
		public readonly float capacity;


		public float capacityFree => capacity - available;

		//public float maxOutput = 1;
		//public float maxinput = 1;

		// Start is called before the first frame update
		void Start()
		{
		
		}

		// Update is called once per frame
		void FixedUpdate()
		{
		
		}

		public override void AddToNetwork()
		{
			manager.storages.Add(this);
		}

		public override void RemoveFromNetwork()
		{
			manager.storages.Remove(this);
		}

	}
}
