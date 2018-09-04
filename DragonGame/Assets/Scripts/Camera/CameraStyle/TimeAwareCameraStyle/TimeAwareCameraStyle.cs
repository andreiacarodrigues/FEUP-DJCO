using UnityEngine;

public abstract class TimeAwareCameraStyle : CameraStyle
{
	private readonly float transitionTime;
	private float timeElapsed = 0;
	protected float TimeRatio { get; private set; }

	protected TimeAwareCameraStyle(Transform cameraTransform, float transitionTime) : base(cameraTransform)
	{
		this.transitionTime = transitionTime;
	}

	public override void Update()
	{
		if(!NewPosition)
		{
			timeElapsed += Time.deltaTime;
			TimeRatio = transitionTime <= 0 ? 1 : timeElapsed / transitionTime;
			Transition();
			if(TimeRatio >= 1)
			{
				NewPosition = true;
			}
		}

		AfterUpdate();
	}

	protected virtual void Transition()
	{

	}

	protected virtual void AfterUpdate()
	{

	}
}