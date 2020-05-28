using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : Resource
{
	public float specificGasConstant = 287.05f; // specific gas constant is measured in j/Kg Kelvins.
	public Air()
	{
		name = "Air";

		//usePressure = true;

		density = 1.2f;
	}

}
