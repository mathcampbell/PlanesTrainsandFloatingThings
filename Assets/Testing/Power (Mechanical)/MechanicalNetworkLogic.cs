using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MechanicalNetworkLogic
{

	static public HashSet<MechanicalNetworkItem>TraverseConnectedNetwork(MechanicalNetworkItem root)
	{
		var openSet = new HashSet<MechanicalNetworkItem>() { root };
		var closedSet = new HashSet<MechanicalNetworkItem>();

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