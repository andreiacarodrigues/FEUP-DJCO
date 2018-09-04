using UnityEngine;

namespace DefaultNamespace
{
	public class SlidingBarWithoutText : SlidingBar
	{
		protected override void HandleBar()
		{
			if (curValue >= 0 && curValue <= maxValue)
			{
				fillAmount = curValue / maxValue;

				bar.fillAmount = Mathf.Lerp (bar.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
			}
		}
	}
}