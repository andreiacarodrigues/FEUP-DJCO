using UnityEngine;

public class MouseControlledCameraStyle : CameraStyle
{
	protected readonly Camera Camera;
	protected readonly Transform ActorTransform;
	protected Vector3 ActorOffset;

	protected static float DefaultXRotationSensitivity = 1;
	protected static float DefaultZoomSensitivity = 10;
	protected static float DefaultMinFieldOfView = 40;
	protected static float DefaultMaxFieldOfView = 70;

	protected readonly float XRotationSensitivity = DefaultXRotationSensitivity;
	protected readonly float ZoomSensitivity = DefaultZoomSensitivity;
	protected readonly float MinFieldOfView = DefaultMinFieldOfView;
	protected readonly float MaxFieldOfView = DefaultMaxFieldOfView;

	public MouseControlledCameraStyle(Camera camera, Transform actorTransform, Vector3 initialOffset, Vector3 lookDirection) : base(camera.transform)
	{
		ActorTransform = actorTransform;
		Camera = camera;

		CameraTransform.position = ActorTransform.position + initialOffset;
		CameraTransform.rotation = Quaternion.LookRotation(lookDirection);

		ActorOffset = initialOffset;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public override void Update()
	{
		float xRotation = Input.GetAxis("Mouse X") * XRotationSensitivity;
		float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * ZoomSensitivity;

		ActorOffset = Quaternion.AngleAxis(xRotation, Vector3.up) * ActorOffset;
		Vector3 newLookVector = Quaternion.AngleAxis(xRotation, Vector3.up) * CameraTransform.forward;

		CameraTransform.position = ActorTransform.position + ActorOffset;
		CameraTransform.rotation = Quaternion.LookRotation(newLookVector);

		Camera.fieldOfView += zoomAmount;
		Camera.fieldOfView = Mathf.Clamp(Camera.fieldOfView, MinFieldOfView, MaxFieldOfView);

		NewPosition = true;
	}
}