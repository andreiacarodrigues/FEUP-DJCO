using UnityEngine;

public class MouseControlledCameraStyleTrigger : MonoBehaviour
{
	public GameObject Actor;
	public Vector3 Offset;
	public Vector3 LookVector;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == Actor)
		{
			CameraScript.CameraScriptObject.SetMouseControlledStyle(Actor.transform, Offset, LookVector);
		}
	}
}