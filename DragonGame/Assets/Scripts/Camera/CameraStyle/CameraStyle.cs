using UnityEngine;

public abstract class CameraStyle
{
	protected Vector3 OldLookVector;
	protected Vector3 OldPosition;
	protected readonly Transform CameraTransform;
	public bool NewPosition { get; protected set; }

	protected CameraStyle(Transform cameraTransform)
	{
		OldLookVector = cameraTransform.forward.normalized;
		OldPosition = cameraTransform.position;
		CameraTransform = cameraTransform;
		NewPosition = false;
		Cursor.lockState = CursorLockMode.None;
	}

	public virtual void Update()
	{

	}

	public bool GetIfNewPosition()
	{
		bool ret = NewPosition;
		NewPosition = false;
		return ret;
	}
}

