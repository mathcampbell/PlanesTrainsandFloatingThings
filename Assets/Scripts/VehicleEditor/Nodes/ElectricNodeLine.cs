using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vehicle.ElectricalPower;

public class ElectricNodeLine : MonoBehaviour
{
	public PowerNetworkItem ConnectedFrom;
	public PowerNetworkItem ConnectedTo;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public bool IsConnected()
	{
		if (ConnectedTo && ConnectedFrom)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

}
