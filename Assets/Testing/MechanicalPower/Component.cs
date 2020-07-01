using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public abstract class Component : MonoBehaviour
{
	protected Network network;


	/// <summary>
	/// Friction losses of the component. (Torque per RPM)
	/// </summary>
	public float frictionLoss;


	public abstract void ShaftUpdate();
}