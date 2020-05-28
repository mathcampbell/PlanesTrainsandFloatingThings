using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helium : Resource
{
	public float specificGasConstant = 2007f; // specific gas constant is measured in j/Kg Kelvins.
	public Helium()
	{
		name = "Helium";

		//usePressure = true;

		density = 0.1786f;
	}

}
