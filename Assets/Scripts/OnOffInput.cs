using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffInput : Datanode
{   
	public  Datanode connectedNode;
	public bool inputIO;

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
