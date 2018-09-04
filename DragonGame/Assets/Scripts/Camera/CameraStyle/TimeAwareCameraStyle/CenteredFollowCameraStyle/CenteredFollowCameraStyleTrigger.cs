using UnityEngine;

public class CenteredFollowCameraStyleTrigger : MonoBehaviour
{
	public GameObject Actor;
	public Vector3 Offset;
	public float TransitionTime;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == Actor)
		{
			CameraScript.CameraScriptObject.SetCenteredFollowStyle(Actor.transform, Offset, TransitionTime);
		}
	}
}