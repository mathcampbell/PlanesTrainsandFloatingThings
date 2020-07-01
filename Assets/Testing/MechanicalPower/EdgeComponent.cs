using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A <see cref="Component"/> that can is connected to more than one <see cref="Network"/>(s).
/// </summary>
public abstract class EdgeComponent : Component
{
	public override void ShaftUpdate()
	{
		throw new InvalidOperationException(nameof(EdgeComponent) + " (and deriving classes) must use the overload that includes a " + nameof(SuperNetwork) + ".");
	}

	/// <summary>
	/// For the perspective of <paramref name="activeNetwork"/> update the component.
	/// Beware that this will be called for each <see cref="Network"/> the <see cref="Component"/> is part of.
	/// </summary>
	/// <param name="activeNetwork"></param>
	public abstract void ShaftUpdate(SuperNetwork activeNetwork);

	/// <summary>
	/// For the perspective of <paramref name="activeNetwork"/>, get the Networks that are currently connected.
	/// </summary>
	/// <param name="activeNetwork"></param>
	/// <returns></returns>
	public abstract List<Network> CurrentlyConnectedNetworks(SuperNetwork activeNetwork);

}