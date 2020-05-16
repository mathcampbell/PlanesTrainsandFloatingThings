using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPower : PowerStorage
{

    public new readonly float capacity = 75000;

    // Start is called before the first frame update
    void Start()
    {
        // Capacity, is in watts - 1W = 1J/per second. This battery will power 20W of power for about one hour. 
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    void FixedUpdate()
    {
        
    }
}
