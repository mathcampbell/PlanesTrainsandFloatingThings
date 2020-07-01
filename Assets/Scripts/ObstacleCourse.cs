using UnityEngine;
using System.Collections.Generic;

public class ObstacleCourse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SpawnCourse();
	}

	public GameObject CheckpointPrefab;

	public int NumCheckpoints = 10;

	public GameObject ObstacleCourseStartPositionObject;

	public Material MatCheckpointInactive;
	public Material MatCheckpointActive;
	public Material MatCheckpointPassed;

	float MinDistance = 50;
	float MaxDistance = 100;
	float MaxAngle = 10;

	List<Checkpoint> checkpoints;
	int activeCheckpointIndex = 0;

	// Update is called once per frame
	void Update () {

	}

	void SpawnCourse()
	{
		checkpoints = new List<Checkpoint>();
		activeCheckpointIndex = 0;

		Vector3 cpPos = ObstacleCourseStartPositionObject.transform.position;
		Quaternion cpRot = Quaternion.identity;

		for (int i = 0; i < NumCheckpoints; i++)
		{
			GameObject cpGO = (GameObject)Instantiate(CheckpointPrefab);
			cpGO.transform.SetParent(this.transform);
			Checkpoint cp = cpGO.GetComponent<Checkpoint>();
			cp.ObstacleCourse = this;
			checkpoints.Add(cp);

			// Position the checkpoint in an interesting manner
			Vector3 offset = new Vector3(0, 0, Random.Range(MinDistance, MaxDistance));

			cpRot *= Quaternion.Euler(
				Random.Range(-MaxAngle, MaxAngle),
				Random.Range(-MaxAngle, MaxAngle),
				0 );

			cpPos += cpRot * offset;

			cpGO.transform.position = cpPos;
			cpGO.transform.rotation = cpRot;
		}

		ActivateCheckpoint( checkpoints[activeCheckpointIndex] );
	}

	void Cleanup()
	{
		// Delete old checkpoints
		if(checkpoints != null)
		{
			foreach(Checkpoint cp in checkpoints)
			{
				Destroy(cp.gameObject);
			}

			checkpoints = null;
		}
	}

	public void CheckpointWasTriggered( Checkpoint cp )
	{
		// Is this the active checkpoint?
		int thisIndex = checkpoints.IndexOf(cp);

		if(thisIndex != activeCheckpointIndex)
		{
			// This is not the active checkpoint
			return;
		}

		InactivateCheckpoint( cp );
		activeCheckpointIndex++;

		if(activeCheckpointIndex >= NumCheckpoints)
		{
			// We have finished!!
			Debug.Log("Course is done.");
			return;
		}

		ActivateCheckpoint( checkpoints[activeCheckpointIndex] );
	}

	void InactivateCheckpoint( Checkpoint cp )
	{
		MeshRenderer[] mrs = cp.transform.GetComponentsInChildren<MeshRenderer>();

		foreach(MeshRenderer mr in mrs)
		{
			mr.material = MatCheckpointPassed;
		}

	}

	void ActivateCheckpoint( Checkpoint cp )
	{
		// Change the material

		MeshRenderer[] mrs = cp.transform.GetComponentsInChildren<MeshRenderer>();

		foreach(MeshRenderer mr in mrs)
		{
			mr.material = MatCheckpointActive;
		}
	}

}
