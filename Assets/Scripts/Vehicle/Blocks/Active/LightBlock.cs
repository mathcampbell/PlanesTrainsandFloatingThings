using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LightBlock : ActiveBlock
{
	public OnOffInput IOInputLightSwitch;
	public bool lightOn;
	public Light LightObject;

	public Material lightMatOriginal;
	private Material lightMat;
	private float powerAvailable;
	public Animator lightBlockAnim;

    
	// Start is called before the first frame update
	void Start()
	{
		lightOn = false;
		LightObject = this.GetComponentInChildren<Light>();
		//lightMat = new Material(lightMatOriginal);
		// lightMat = lightMatOriginal;
		//lightMat.DisableKeyword("_EMISSION");
		LightObject.intensity = 0f;
		lightBlockAnim = GetComponent<Animator>();
		lightBlockAnim.SetBool("BlockLight", false);
	}

	override public void Init()
	{
	   // this.GetComponentInChildren<Renderer>().material = lightMat;

	}
	// Update is called once per frame
	void Update()
	{
		
	}

	private void FixedUpdate() {
		if (IOInputLightSwitch.inputIO)
		{
            GetComponentInChildren<GenericConsumer>().requested = 0.1f;
			powerAvailable = GetComponentInChildren<GenericConsumer>().recieved;
			float powerFraction = powerAvailable / 0.1f;
            
				if (lightOn)
                {
					SetLightLevel(powerFraction);
					return;
                }
					

				else if (lightOn == false)
                {
				if (powerFraction > 0.2f)
				{
					TurnLightOn();
					lightOn = true;
					SetLightLevel(powerFraction);
				}
				else
					return;
                }
		

		}
		else
		{
			GetComponentInChildren<GenericConsumer>().requested = 0f;
			TurnLightOff();
			lightOn = false;
		}
	}

	void TurnLightOn()
	{
		//this.GetComponentInChildren<Renderer>().material = lightMat;
		//lightMat.EnableKeyword("_EMISSION");
		lightBlockAnim.SetBool("BlockLight", true); 

	}

	void TurnLightOff()
	{
		//this.GetComponentInChildren<Renderer>().material = lightMat;
		LightObject.intensity = 0f;
		//lightMat.DisableKeyword("_EMISSION");
		lightBlockAnim.SetBool("BlockLight", false);
	}

	void SetLightLevel(float powerFraction)
    {
		LightObject.intensity = (0.2f * powerFraction);

		if (powerFraction < 0.2f)
			{
			TurnLightOff();
			lightOn = false;
			GetComponentInChildren<GenericConsumer>().requested = 0f;
		}
    }
}
