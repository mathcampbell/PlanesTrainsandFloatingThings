using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPower : PowerConsumer
{

    public float requested;
    public float recieved;

    // Start is called before the first frame update
    void Start()
    {
         

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    void FixedUpdate()
    {

    }


    public override float  PotentialConsumption()
    {
        return requested;
    }

    public override void  Consume(float fractionOfPotential)
    {
        recieved = fractionOfPotential;
    }
    



}
