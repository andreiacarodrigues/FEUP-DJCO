using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinesController : MonoBehaviour
{
	void Start ()
    {
	    if(SaveLoadController.data != null && SaveLoadController.data.VinesDestroyed == true)
        {
            DestroyVines();
        }
	}

    public void DestroyVines()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        SaveLoadController.VinesDestroyedStatic = true;
    }
}