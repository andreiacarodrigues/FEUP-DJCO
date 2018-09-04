using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameController : MonoBehaviour {

	PauseMenuController pc;
	private ArrayList storyLine = new ArrayList();
	public TextMeshProUGUI scrollText;
	private bool isTyping = false;
	private bool cancelTyping = false;
	private float typeSpeed = .03f;

	void Start () 
	{
		pc = GameObject.FindObjectOfType<PauseMenuController> ();

		cancelTyping = false;
		isTyping = false;

		storyLine.Add("Congratulations!\n" +
			"You saved the princess and her kingdom!\n" +
			"Thanks to your bravery, you will live in the castle as the princess's royal guard.\n" +
			"With the end of this adventure there is only one thing to say:\n 'And they lived happily ever after'!\n");

		StartCoroutine (TextScroll ((string)storyLine [0]));
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.E) && isTyping && !cancelTyping) 
		{   
			cancelTyping = true;
		} 
		else if (Input.GetKeyDown (KeyCode.E) && !isTyping) 
		{
			pc.ExitToMenu ();
		}

	}

	private IEnumerator TextScroll(string text)
	{
		int letter = 0;
		scrollText.text = "";
		isTyping = true;
		cancelTyping = false;

		while (isTyping && !cancelTyping && (letter < text.Length - 1)) 
		{
			scrollText.text += text [letter];
			letter++;
			yield return new WaitForSeconds (typeSpeed);
		}
		scrollText.text = text;
		isTyping = false;
		cancelTyping = false;
	}
}
