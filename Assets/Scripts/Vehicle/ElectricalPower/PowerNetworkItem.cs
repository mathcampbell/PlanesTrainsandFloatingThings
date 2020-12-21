using System.Collections.Generic;

using UnityEngine;

using Vehicle.ElectricalPower.PowerTypes;

namespace Vehicle.ElectricalPower {
	/// <summary>
	/// Item in a power network
	/// </summary>
	public abstract class PowerNetworkItem : MonoBehaviour
	{
		/// <summary>
		/// The network manager this item is part of.
		/// </summary>
		public PowerNetworkManager manager;

		public List<PowerNetworkItem> DirectlyConnected;

		/// <summary>
		/// The power type.
		/// </summary>
		PowerType powerType => manager.powerType;


		// Start is called before the first frame update
		void Start()
		{
		
		}

		// Update is called once per frame
		void FixedUpdate()
		{
		
		}

		public abstract void AddToNetwork();

		public abstract void RemoveFromNetwork();
	
	}
}
