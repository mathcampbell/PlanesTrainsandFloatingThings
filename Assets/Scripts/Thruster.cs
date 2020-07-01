using UnityEngine;
using System.Collections;

public class Thruster : KeybindableComponent {

	// Use this for initialization
	void Start () {
		shipRigidbody = this.transform.root.GetComponent<Rigidbody>();
		if(shipRigidbody == null)
		{
			Debug.LogError("No rigidbody?");
		}

	}

	public float ThrusterStrength = 10f;

	Rigidbody shipRigidbody;


	// FixedUpdate runs immediately before each tick of the physics engine.
	// This is where all changes to physics should happen.
	// (Exception: Totally instantaneous physical effects CAN be done in normal
	// Update(), but it's probably less confusing if you still do them in Fixed.)
	void FixedUpdate()
	{
		if(shipRigidbody.isKinematic == true)
		{
			// Kinematic is on, so no physics, so no thrusting.
			SetParticles(false);
			return;
		}

		if( Input.GetKey(keyCode) )
		{
			// Thrust is a go!
			Vector3 theForce = -this.transform.forward * ThrusterStrength;

			shipRigidbody.AddForceAtPosition( theForce, this.transform.position );

			SetParticles(true);

		}
		else
		{
			// Not thrusting!
			SetParticles(false);
		}

	}

	void SetParticles(bool enabled)
	{
		ParticleSystem.EmissionModule em = GetComponentInChildren<ParticleSystem>().emission;
		em.enabled = enabled;
	}

	// Update is called once per VISUAL frame
	void Update () {

	}
}
