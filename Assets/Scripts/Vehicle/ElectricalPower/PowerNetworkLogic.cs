using System.Collections.Generic;
using System.Linq;

namespace Vehicle.ElectricalPower {
	public static class PowerNetworkLogic
	{

		public static HashSet<PowerNetworkItem>TraverseConnectedNetwork(PowerNetworkItem root)
		{
			var openSet = new HashSet<PowerNetworkItem>() { root };
			var closedSet = new HashSet<PowerNetworkItem>();

			while (openSet.Count > 0)
			{
				var current = openSet.First();

				foreach (var child in current.DirectlyConnected.Where(x => !closedSet.Contains(x)))
					openSet.Add(child);

				openSet.Remove(current);
				closedSet.Add(current);
			}

			return closedSet;
		}

	}
}