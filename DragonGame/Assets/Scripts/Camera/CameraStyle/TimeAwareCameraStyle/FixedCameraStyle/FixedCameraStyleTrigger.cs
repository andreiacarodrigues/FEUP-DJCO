using UnityEngine;

public class FixedCameraStyleTrigger : MonoBehaviour
{
	public GameObject Actor;
	public GameObject TargetTransform;
	public float TransitionTime;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == Actor)
		{
			CameraScript.CameraScriptObject.SetNewFixedTarget(TargetTransform.transform, TransitionTime);
		}
	}
}