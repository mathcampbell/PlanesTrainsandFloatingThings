using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Testing.MechanicalPower
{
/// <summary>
/// A <see cref="ShaftComponent"/> that links two <see cref="ShaftNetwork"/>s with a selectable ratio between them
/// </summary>
public class GearBox : ShaftEdgeComponent
{
	protected ShaftNetwork network2;

	int selectedRatioIndex;
	List<float> ratios;




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
