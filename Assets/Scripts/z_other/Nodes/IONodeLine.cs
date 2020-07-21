using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IONodeLine : MonoBehaviour
{
    public OnOffInput ConnectedInput;
    public OnOffOutput ConnectedOutput;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsConnected()
    {
        if (ConnectedInput && ConnectedOutput)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
