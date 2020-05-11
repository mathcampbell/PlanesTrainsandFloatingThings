using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : ActiveBlock
{
	public OnOffOutput IOOutputSwitch;
	

	public Material PushButtonKeyOriginalMat;
	private Material PushButtonKeyMat;
	// Start is called before the first frame update
	void Start()
	{
		
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

	private void FixedUpdate() {
		

	}

	void TurnSwitchOn()
	{
		this.GetComponentInChildren<Renderer>().material = PushButtonKeyMat;
		
		PushButtonKeyMat.EnableKeyword("_EMISSION");
	

	}

	void TurnSwitchOff()
	{
		this.GetComponentInChildren<Renderer>().material = PushButtonKeyMat;
		
		PushButtonKeyMat.DisableKeyword("_EMISSION");
	}
}
