using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	[System.NonSerialized]
	public ObstacleCourse ObstacleCourse;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter()
	{
		//Debug.Log(gameObject.name + " was triggered.");
		ObstacleCourse.CheckpointWasTriggered(this);
	}
}
