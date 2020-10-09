using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PressureManager : Monobehaviour

{
	public VehicleController vehicle;


	public void Start()
	{ }

	public void Awake()
	{ }

	public void Update()
    {

		// We'll need to add event notification stuff here so that when a block is damaged or something changes like a door opens, the pressure system is asked to update.
    }

	public void PressureUpdate()
    {


    }
}
