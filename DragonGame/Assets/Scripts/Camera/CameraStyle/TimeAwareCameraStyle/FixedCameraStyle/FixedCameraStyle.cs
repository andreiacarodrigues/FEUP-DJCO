using UnityEngine;

public class FixedCameraStyle : TimeAwareCameraStyle
{
	private readonly Vector3 targetLookVector;
	private readonly Vector3 targetPosition;

	public FixedCameraStyle(Transform cameraTransform, Vector3 targetLookVector, Vector3 targetPosition,
		float transitionTime) : base(cameraTransform, transitionTime)
	{
		this.targetLookVector = targetLookVector;
		this.targetPosition = targetPosition;
	}

	protected override void Transition()
	{
		CameraTransform.rotation = Quaternion.Slerp(Quaternion.LookRotation(OldLookVector), Quaternion.LookRotation(targetLookVector), TimeRatio);
		CameraTransform.position = Vector3.Lerp(OldPosition, targetPosition, TimeRatio);
	}
}