using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class of all resources.
/// </summary>
//This continas shared functinality
public abstract class Resource
{
	/// <summary>
	/// The name of the resource
	/// </summary>
	public string name;


	/// <summary>
	/// Does this resource use pressure calculation
	/// </summary>
	//public bool usePressure = false;
	// Density is the mass in kg per m^3; Water is 1000 exactly. Seawater is around 1030 but lets not get that precise ;)
    // Might also need the specific gas constant here as well/instead?
	
    public float density;
 
}
