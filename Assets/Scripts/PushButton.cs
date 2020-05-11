using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : ActiveBlock
{
    public OnOffOutput IOOutputSwitch;


    public Material PushButtonKeyOriginalMat;
    private Material PushButtonKeyMat;

    private Animator ButtonPushedAnim;

    private bool SwitchIsOn;

    // Start is called before the first frame update
    void Start()
    {
        ButtonPushedAnim = GetComponent<Animator>();
        PushButtonKeyMat = new Material(PushButtonKeyOriginalMat);
        PushButtonKeyMat.DisableKeyword("_EMISSION");

    }

    override public void Init()
    {
        // this.GetComponentInChildren<Renderer>().material = lightMat;

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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, BlockLogic.LayerMaskBlock))
            {
                var hitcollider = this.GetComponent<Collider>();
                if (hitInfo.collider == hitcollider)
                {
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
        this.GetComponentInChildren<Renderer>().material = PushButtonKeyMat;

        PushButtonKeyMat.EnableKeyword("_EMISSION");

        ButtonPushedAnim.SetBool("Pressed", true);
        SwitchIsOn = true;


    }

    void TurnSwitchOff()
    {
        this.GetComponentInChildren<Renderer>().material = PushButtonKeyMat;

        PushButtonKeyMat.DisableKeyword("_EMISSION");
        ButtonPushedAnim.SetBool("Pressed", false);
        SwitchIsOn = false;
    }
}
