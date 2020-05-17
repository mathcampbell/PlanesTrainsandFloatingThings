using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PowerNetworkLogic
{

    static public HashSet<PowerNetworkItem>TraverseConnectedNetwork(PowerNetworkItem root)
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