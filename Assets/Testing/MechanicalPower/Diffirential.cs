using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
/// <summary>
/// A <see cref="ShaftComponent"/> that represents a diffirential.
/// </summary>
public class Diffirential : ShaftEdgeComponent
{
	ShaftNetwork outputLeft;
	ShaftNetwork outputRight;

	// Diffirentials commonly also have a fixed gear ratio between input and output side.
	float gearRatio;



	public override List<ShaftNetwork> CurrentlyConnectedNetworks(ShaftNetworkGroup activeNetwork)
	{
		throw new NotImplementedException();
	}

	public override void ShaftUpdate(ShaftNetworkGroup activeNetwork)
	{
		throw new NotImplementedException();
	}
}
}
