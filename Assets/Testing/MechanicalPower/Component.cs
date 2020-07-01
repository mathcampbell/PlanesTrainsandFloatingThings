using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// A <see cref="Component"/> repressents any object in a Shaft network that has functionality.
/// </summary>
public abstract class Component : MonoBehaviour
{
	/// <summary>
	/// The network this <see cref="Component"/> is a part of.
	/// </summary>
	protected Network network;


	/// <summary>
	/// Friction losses of the component. (Torque per RPM)
	/// </summary>
	public float frictionLoss;


	/// <summary>
	/// Do shaft related actions.
	/// </summary>
	public abstract void ShaftUpdate();
}