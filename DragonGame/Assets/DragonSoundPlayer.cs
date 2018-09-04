using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSoundPlayer : MonoBehaviour
{
    FMOD.Studio.EventInstance MovementSoundEvent;

	public void PlaySound(AnimationEvent animEvent)
    {
		MovementSoundEvent = FMODUnity.RuntimeManager.CreateInstance(animEvent.stringParameter); 
		MovementSoundEvent.setParameterValue("MovementType", animEvent.intParameter);
		MovementSoundEvent.setParameterValue("SurfaceType", (int)animEvent.floatParameter);
        MovementSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.parent.transform.position));
        MovementSoundEvent.start();

        // FMODUnity.RuntimeManager.PlayOneShot(path, transform.parent.transform.position);
    }

	public void PlaySoundScript(string path, int movement, int surface)
	{
		MovementSoundEvent = FMODUnity.RuntimeManager.CreateInstance(path); 
		MovementSoundEvent.setParameterValue("MovementType", movement);
		MovementSoundEvent.setParameterValue("SurfaceType", surface);
		MovementSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.parent.transform.position));
		MovementSoundEvent.start();

		// FMODUnity.RuntimeManager.PlayOneShot(path, transform.parent.transform.position);
	}
}