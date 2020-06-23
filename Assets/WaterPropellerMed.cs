using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPropellerMed : ActiveBlock
{

    // A bool to check i we're connected to a mechanical power source!

    public float RPM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isConnected)
        {
            // Here we'll run the code tht gets the current power and termines the speed to turn teh rotor (if at all)
        }
    }

    void Update()
    {
        
    }
}
