using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlidingBar : MonoBehaviour {

	[SerializeField]
	protected Image bar;

	[SerializeField]
	protected float lerpSpeed = 1;

	[SerializeField]
	protected TextMeshProUGUI textValue;

	[SerializeField]
	protected float maxValue;

	[SerializeField]
	protected float curValue;

	[SerializeField]
	protected float fillAmount;

	void Start () 
	{
		fillAmount = bar.fillAmount;
	}

	void Update () 
	{
		HandleBar ();
	}

	protected virtual void HandleBar()
	{
		if (curValue >= 0 && curValue <= maxValue) 
		{
			fillAmount = curValue / maxValue;

			bar.fillAmount = Mathf.Lerp (bar.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
			textValue.text = curValue.ToString() + '/' + maxValue.ToString();
		}
	}
		
	public float GetMaxValue()
	{
		return maxValue;
	}

	public void SetMaxValue(float value)
	{
		maxValue = value;
	}

	public float GetCurValue()
	{
		return curValue;
	}

	public void SetCurValue(float value)
	{
		curValue = Mathf.Clamp(value, 0, maxValue);
	}
}
