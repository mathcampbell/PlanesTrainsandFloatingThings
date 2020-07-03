using System;
using System.Collections.Generic;

namespace Assets.Testing.MechanicalPower
{
/// <summary>
/// A <see cref="ShaftComponent"/> that can is connected to more than one <see cref="ShaftNetwork"/>(s).
/// </summary>
public abstract class ShaftEdgeComponent : ShaftComponent
{
	public override void ShaftUpdate()
	{
		throw new InvalidOperationException(nameof(ShaftEdgeComponent) + " (and deriving classes) must use the overload that includes a " + nameof(ShaftNetworkGroup) + ".");
	}

	/// <summary>
	/// For the perspective of <paramref name="activeNetwork"/> update the component.
	/// Beware that this will be called for each <see cref="ShaftNetwork"/> the <see cref="ShaftComponent"/> is part of.
	/// </summary>
	/// <param name="activeNetwork"></param>
	public abstract void ShaftUpdate(ShaftNetworkGroup activeNetwork);

	/// <summary>
	/// For the perspective of <paramref name="activeNetwork"/>, get the Networks that are currently connected.
	/// </summary>
	/// <param name="activeNetwork"></param>
	/// <returns></returns>
	public abstract List<ShaftNetwork> CurrentlyConnectedNetworks(ShaftNetworkGroup activeNetwork);

}
}