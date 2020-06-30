using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public abstract class EdgeComponent : Component
{
	public override void ShaftUpdate()
	{
		throw new InvalidOperationException(nameof(EdgeComponent) + " (and deriving classes) must use the overload that includes a " + nameof(SuperNetwork) + ".");
	}

	/// <summary>
	/// For the perspective of <paramref name="activeNetwork"/> update the component.
	/// </summary>
	/// <param name="activeNetwork"></param>
	public abstract void ShaftUpdate(SuperNetwork activeNetwork);

	/// <summary>
	/// For the perspective of <paramref name="activeNetwork"/>, get the Networks that are currently connected.
	/// </summary>
	/// <param name="activeNetwork"></param>
	/// <returns></returns>
	public abstract List<Network> CurrentleConnectedNetworks(SuperNetwork activeNetwork);

}