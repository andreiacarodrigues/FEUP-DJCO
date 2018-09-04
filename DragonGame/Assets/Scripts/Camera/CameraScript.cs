using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public static CameraScript CameraScriptObject = null;

	private CameraStyle cameraStyle;
	private new Camera camera;

	public Vector3 CurrentForwardProjection { get; private set; }
	public Vector3 CurrentRightProjection { get; private set; }

	private bool moveCamera;

	// Use this for initialization
	private void Start()
	{
		if(CameraScriptObject != null)
		{
			Destroy(gameObject);
		}
		CameraScriptObject = this;
		camera = GetComponent<Camera>();
		cameraStyle = new MouseControlledCameraStyle(camera, GameObject.FindGameObjectWithTag("Player").transform,
			new Vector3(0, 2, 5), new Vector3(0, 0, -1));

		/*cameraStyle = new MouseControlledCameraStyle(transform, GameObject.FindGameObjectWithTag("Player").transform,
			new Vector3(5, 4, 0));*/

		moveCamera = true;
	}

	// Update is called once per frame
	private void Update()
	{
		if (moveCamera) 
		{
			cameraStyle.Update ();
			if (cameraStyle.GetIfNewPosition ()) 
			{
				CurrentForwardProjection = Vector3.ProjectOnPlane (transform.forward, Vector3.up).normalized;
				if (CurrentForwardProjection == Vector3.zero) 
				{
					CurrentForwardProjection = transform.up;
				}

				CurrentRightProjection = (Quaternion.Euler (0, 90, 0) * CurrentForwardProjection).normalized;
			}
		}
	}

	public void SetNewCameraStyle(CameraStyle newCameraStyle)
	{
		cameraStyle = newCameraStyle;
	}

	public void SetNewFixedTarget(Vector3 lookVector, Vector3 position, float newTransitionTime)
	{
		SetNewCameraStyle(new FixedCameraStyle(transform, lookVector, position, newTransitionTime));
	}

	public void SetNewFixedTarget(Transform targetTransform, float newTransitionTime)
	{
		SetNewCameraStyle(new FixedCameraStyle(transform, targetTransform.forward, targetTransform.position,
			newTransitionTime));
	}

	public void SetCenteredFollowStyle(Transform actorTransform, Vector3 actorOffset, float transitionTime)
	{
		SetNewCameraStyle(new CenteredFollowCameraStyle(transform, actorTransform, actorOffset, transitionTime));
	}

	public void SetMouseControlledStyle(Transform actorTransform, Vector3 actorOffset, Vector3 lookVector)
	{
		SetNewCameraStyle(new MouseControlledCameraStyle(camera, actorTransform, actorOffset, lookVector));
	}

	public void SetMoveCamera(bool newState)
	{
		moveCamera = newState;
	}
}