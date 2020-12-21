using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(TheCamera == null)
		{
			// Do we have a camera component?
			TheCamera = GetComponent<Camera>();
		}

		if(TheCamera == null)
		{
			// Is there a main camera?
			TheCamera = Camera.main;
		}

		if(TheCamera == null)
		{
			Debug.LogError("Could not find a camera.");
			return;
		}

		cameraRig = TheCamera.transform.parent;
	}

	public Camera TheCamera;
	private Transform cameraRig;
	private Vector3 lastMousePos;

	public float OrbitSensitivity = 10;
	public bool HoldToOrbit = false;

	public float ZoomMultiplier = 2;
	public float minDistance = 2;
	public float maxDistance = 25;
	public bool InvertZoomDirection = false;
	public float PanSpeed = 0.1f;

	// Update is called once per frame
	void Update()
	{
		OrbitCamera();
		DollyCamera();
		PanCamera();
		CraneCamera();
	}


	void CraneCamera()
	{
		float craneThisFrame = 0f;
		if (Input.GetKey("e"))
		{
			craneThisFrame = -0.5f;
		}
		else if (Input.GetKey("q"))
		{
			craneThisFrame = 0.5f;
		}

		Vector3 input = new Vector3( 0, craneThisFrame, 0);

		Vector3 actualCrane = input * PanSpeed;

		Vector3 newPosition = cameraRig.transform.position + actualCrane;
		cameraRig.transform.position = newPosition;
	}
	void PanCamera()
	{
		Vector3 input = new Vector3( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical") );

		Vector3 actualChange = input * PanSpeed;

		actualChange = Quaternion.Euler( 0, TheCamera.transform.rotation.eulerAngles.y, 0 ) * actualChange;

		Vector3 newPosition = cameraRig.transform.position + actualChange;

		cameraRig.transform.position = newPosition;

	}

	void DollyCamera()  // a.k.a "Zoom" camera
	{
		float delta = -Input.GetAxis("Mouse ScrollWheel");
		if(InvertZoomDirection)
			delta = -delta;

		// Move the camera backwards or forwards based on the value of delta

		Vector3 actualChange = TheCamera.transform.localPosition * ZoomMultiplier * delta;

		Vector3 newPosition = TheCamera.transform.localPosition + actualChange;

		newPosition = newPosition.normalized * Mathf.Clamp( newPosition.magnitude, minDistance, maxDistance );

		TheCamera.transform.localPosition = newPosition;
	}


	void OrbitCamera ()
	{
		if(Input.GetMouseButtonDown(1) == true)
		{
			// The mouse was pressed ON THIS FRAME
			lastMousePos = Input.mousePosition;
		}

		if( Input.GetMouseButton(1) == true )
		{
			// We are currently holding down the right mouse button

			// What is the current position of the mouse on the screen?
			Vector3 currentMousePos = Input.mousePosition;

			// In pixels!
			Vector3 mouseMovement = currentMousePos - lastMousePos;

			// Let's "orbit" the camera rig with our actual camera object
			// When we orbit, the distance from the rig stays the same, but
			// the angle changes.  Or another way to put, we want to rotate
			// the vector indicating the relative position of our camera from
			// the rig

			Vector3 posRelativeToRig = TheCamera.transform.localPosition;

			Vector3 rotationAngles = mouseMovement / OrbitSensitivity;

			if(HoldToOrbit)
			{
				rotationAngles *= Time.deltaTime;
			}

			// TODO: Fix me
			//Quaternion theOrbitalRotation = Quaternion.Euler( rotationAngles.y, rotationAngles.x, 0 );

			//posRelativeToRig = theOrbitalRotation * posRelativeToRig;

			TheCamera.transform.RotateAround( cameraRig.position, TheCamera.transform.right, -rotationAngles.y  );
			TheCamera.transform.RotateAround( cameraRig.position, TheCamera.transform.up, rotationAngles.x  );

			//cameraRig.Rotate( theOrbitalRotation.eulerAngles, Space.Self );

			// Make sure our camera is still looking are our focal point (i.e. the rig)

			Quaternion lookRotation = Quaternion.LookRotation( -TheCamera.transform.localPosition );
			TheCamera.transform.rotation = lookRotation;

			if( HoldToOrbit == false )
			{
				lastMousePos = currentMousePos;
			}
		}
	}
}
