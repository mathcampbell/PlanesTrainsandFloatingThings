using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public abstract class Component : MonoBehaviour
{
	protected Network network;



	public abstract void ShaftUpdate();
}