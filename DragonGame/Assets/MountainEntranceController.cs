using UnityEngine;

public class MountainEntranceController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            FindObjectOfType<VinesController>().DestroyVines();
            Destroy(gameObject);
        }
    }
}