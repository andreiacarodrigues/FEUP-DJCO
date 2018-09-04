using UnityEngine;

public class CenteredFollowCameraStyle : TimeAwareCameraStyle
{
	private readonly Transform actorTransform;
	private readonly Vector3 targetActorOffset;
	private readonly Vector3 targetLookVector;
	private Vector3 currentActorOffset;
	private readonly Vector3 oldActorOffset;

	public CenteredFollowCameraStyle(Transform cameraTransform, Transform actorTransform, Vector3 targetActorOffset, float transitionTime) : base(
		cameraTransform, transitionTime)
	{
		this.actorTransform = actorTransform;
		this.targetActorOffset = targetActorOffset;
		targetLookVector = (-targetActorOffset).normalized;
		oldActorOffset = currentActorOffset = cameraTransform.position - actorTransform.position;
	}

	protected override void Transition()
	{
		CameraTransform.rotation = Quaternion.Slerp(Quaternion.LookRotation(OldLookVector), Quaternion.LookRotation(targetLookVector), TimeRatio);
		currentActorOffset = Vector3.Lerp(oldActorOffset, targetActorOffset, TimeRatio);
	}

	protected override void AfterUpdate()
	{
		CameraTransform.position = actorTransform.position + currentActorOffset;
	}
}