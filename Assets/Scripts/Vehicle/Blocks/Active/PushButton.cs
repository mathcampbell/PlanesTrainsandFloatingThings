using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : ActiveBlock
{
    public OnOffOutput IOOutputSwitch;


    

    private Animator PushButtonAnim;

    private bool SwitchIsOn;

    // Start is called before the first frame update
    void Start()
    {
        PushButtonAnim = GetComponent<Animator>();
        
        

    }

    override public void Init()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

        IOOutputSwitch.ouputIO = SwitchIsOn;
        if (Input.GetKey(KeyCode.F))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, BlockLogic.LayerMaskBlock))
            {
                var hitcollider = this.GetComponent<Collider>();
                if (hitInfo.collider == hitcollider && !SwitchIsOn)
                {
                    Debug.Log("Switch turning on");
                    TurnSwitchOn();
                }
            }
        }
        else
        {
            TurnSwitchOff();
        }
    }

    void TurnSwitchOn()
    {
        PushButtonAnim.SetBool("Pressed", true);
        SwitchIsOn = true;
    }

    void TurnSwitchOff()
    {    
        PushButtonAnim.SetBool("Pressed", false);
        SwitchIsOn = false;
    }
}
