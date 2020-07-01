using UnityEngine;
using System.Collections;

public class DustField : MonoBehaviour {

	public int NumDustMotes = 100;

	public GameObject DustMotePrefab;

	public Transform TheCamera;

	public float CloudRadius = 25;

	// Use this for initialization
	void Start () {

		if(DustMotePrefab == null)
		{
			Debug.LogError("You forgot to link the dust mote prefab, silly.");
			return;
		}

		if(TheCamera == null)
		{
			if(Camera.main != null)
			{
				TheCamera = Camera.main.transform;
			}

			if(TheCamera == null)
			{
				Debug.LogError("Couldn't find the camera.");
				return;
			}
		}

		MeshRenderer mr = DustMotePrefab.transform.GetComponentInChildren<MeshRenderer>();
		Material matSpaceDust = mr.sharedMaterial;
		matSpaceDust.SetFloat("_FalloffDistance", CloudRadius);
		//mr.sharedMaterial =

		for (int i = 0; i < NumDustMotes; i++)
		{
			Vector3 dustMotePosition = TheCamera.transform.position +
				(Random.insideUnitSphere * CloudRadius);

			Instantiate(DustMotePrefab, dustMotePosition, Random.rotation, this.transform);
		}

	}

	// Update is called once per frame
	void Update () {

		// If any dust mote gets too far from the camera,
		// reposition it to the other side of the dust cloud "sphere"

		float maxDistanceSquared = CloudRadius * CloudRadius;

		for (int i = 0; i < this.transform.childCount; i++)
		{
			// Is this child too far away from the camera?
			Transform theChild = this.transform.GetChild(i);
			Vector3 diff = theChild.position - TheCamera.position;

			if(diff.sqrMagnitude > maxDistanceSquared)
			{
				// Yes, it's too far!
				// So let's flip the dust mote to the other side of the camera.
				diff = Vector3.ClampMagnitude(diff, CloudRadius);
				Vector3 newPosition = TheCamera.position - diff;
				theChild.position = newPosition;
			}
		}
	}
}
