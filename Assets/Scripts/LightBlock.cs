using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlock : ActiveBlock
{
	public OnOffInput IOInputLightSwitch;
	public bool lightOn;
	public Light LightObject;

	public Material lightMatOriginal;
	private Material lightMat;
	private float powerAvailable;
    
	// Start is called before the first frame update
	void Start()
	{
		lightOn = false;
		LightObject = this.GetComponentInChildren<Light>();
		lightMat = new Material(lightMatOriginal);
		// lightMat = lightMatOriginal;
		lightMat.DisableKeyword("_EMISSION");
		LightObject.intensity = 0f;
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
            GetComponentInChildren<LightPower>().requested = 0.1f;
			powerAvailable = GetComponentInChildren<LightPower>().recieved;
			TurnLightOn();
			lightOn = true;

		}
		else
		{
			GetComponentInChildren<LightPower>().requested = 0f;
			TurnLightOff();
			lightOn = false;
		}
	}

	void TurnLightOn()
	{
		this.GetComponentInChildren<Renderer>().material = lightMat;

		LightObject.intensity = (0.2f * powerAvailable);
		lightMat.EnableKeyword("_EMISSION");
		



	}

	void TurnLightOff()
	{
		this.GetComponentInChildren<Renderer>().material = lightMat;
		LightObject.intensity = 0f;
		lightMat.DisableKeyword("_EMISSION");
	}
}
