using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericInput : Datanode
{   
	public  Datanode connectedNode;
	public float inputNumeric;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		transform.LookAt(Camera.main.transform.position, -Vector3.up);
	}

	private void FixedUpdate() 
	{
		

	}
}
