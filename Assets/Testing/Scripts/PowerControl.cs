using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class PowerControl : MonoBehaviour
{

    [SerializeField] public int capacity = 100;

    public int currentCharge;
    public bool dead;
    public bool maxcharged;

    private void Awake()
    {
        currentCharge = capacity;
        dead = false;
        maxcharged = true;
    }

    private void FixedUpdate()
    {
        if (currentCharge < 0)
        {
            dead = true;
            currentCharge = 0;
        }

        if (currentCharge < capacity && maxcharged)
        {
            maxcharged = false;
        }
    }

    private void Update()
    {

    }

    //To drain power call PowerControl.Drain(amount to draw)
    public void Drain(int amount)
    {
        if (!dead)
        {
            currentCharge -= amount;

            if (currentCharge <= 0)
            {
                currentCharge = 0;
                dead = true;
            }
        }
    }

    //To drain power call PowerControl.Charge(amount to add)
    public void Charge(int amount)
    {
        if (!maxcharged)
        {
            currentCharge += amount;

            if (currentCharge >= capacity)
            {
                currentCharge = capacity;
                maxcharged = true;
            }
        }
    }
}

