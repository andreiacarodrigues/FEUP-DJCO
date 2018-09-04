using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryController : MonoBehaviour
{

	private GameObject player;

	/* Controllers */
	private CameraScript cs;
	private MovementScript ms;

	/* Story */
	private ArrayList storyLine = new ArrayList();
	public GameObject scrollPanel;
	public GameObject pressText;
	public GameObject closeBtn;
	private Image scrollImage;
	public TextMeshProUGUI scrollText;

	/* State Booleans */
	private bool isTyping = false;
	private bool cancelTyping = false;
	private float typeSpeed = .03f;
	private float scrollAlphaSpeed = 1.2f;
	private float timeToWaitForScroll = 1f;

	private bool hasWaited = false;

	private static int numberScrollsFound = 0;

	private Color initColor;

	bool fadeIn = false;
	bool showText = false;

	void Start () 
	{
		cs = GameObject.FindObjectOfType<CameraScript> ();
		ms = GameObject.FindObjectOfType<MovementScript> ();
		scrollImage = scrollPanel.GetComponentInChildren<Image> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		initColor = scrollImage.color;

		storyLine.Add("Welcome noble player.\n" +
			"In this adventure you will be in the skin of a little dragon!" +
			"\nWhen exploring this mystical forest you will encounter several enemies with whom you will have to fight, " +
			"grow and learn and do not forget to catch all the scrolls ...\n");

		storyLine.Add("Your story is not the happiest but it can end well.\n" +
			"Your parents made a promise to a princess, a great friend of yours.\n" +
			"To avoid marrying a greedy prince your family promised to protect and hide the beautiful princess in your cave," +
			" so that the prince could not find her.\n");

		storyLine.Add("The prince hoped to marry the princess to restore his kingdom.\n" +
			"Seeing that the princess had disappeared, he sought her all over the kingdom.\n" +
			"He then found your cave.\n" +
			"Your parents tried to protect the princess, but they failed ...\n");

		storyLine.Add("Your parents did not survive the implacable prince.\n" +
			"The princess was then taken to a tower to marry.\n" +
			"The prince did not know there was a third dragon, you.\n" +
			"Then to keep the promise made to your friend and avenge the death of your parents you will have to beat the prince" +
			" in a battle until death.\n");

		// (antes da batalha final) 
		storyLine.Add("It's time to prove your strength.\n" +
			"Slay the prince, save the princess!\n");

		// (no fim da batalha ganha em baixo do win)
		storyLine.Add("Congratulations!\n" +
			"You saved the princess and her kingdom!\n" +
			"Thanks to your bravery, you will live in the castle as the princess's royal guard.\n" +
			"With the end of this adventure there is only one thing to say:\nAnd they lived happily ever after\n");

		// (no fim da batalha perdida em baixo do lose)
		storyLine.Add("Oh no !\n" +
			"Do not give up noble player!\n");

		if (pressText.activeSelf) 
		{
			pressText.SetActive (false);
		}

		if (closeBtn.activeSelf) 
		{
			closeBtn.SetActive (false);
		}

		if (scrollPanel.activeSelf) 
		{
			scrollPanel.SetActive (false);
		}
	}

	void Update()
	{
		if (!scrollPanel.activeSelf)
			return;

		if (Input.GetKeyDown (KeyCode.E) && isTyping && !cancelTyping)
        {   
            cancelTyping = true;
		}

		if (fadeIn) 
		{
			Color c = scrollImage.color;
			c.a = Mathf.Lerp(scrollImage.color.a,scrollAlphaSpeed,Time.deltaTime * 2);
			scrollImage.color = c;
		}

		if (scrollPanel.activeSelf) 
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private IEnumerator TextScroll(string text)
	{
		int letter = 0;
		scrollText.text = "";
		isTyping = true;
		cancelTyping = false;

		if (!hasWaited) 
		{
			hasWaited = true;
			yield return new WaitForSeconds (timeToWaitForScroll);
			pressText.SetActive (true);
			closeBtn.SetActive (true);
		}
			
		while (isTyping && !cancelTyping && (letter < text.Length - 1)) 
		{
			scrollText.text += text [letter];
			letter++;
			yield return new WaitForSeconds (typeSpeed);
		}
		scrollText.text = text;
		isTyping = false;
		cancelTyping = false;
		hasWaited = false;
	}

	public void StoryMode(int scrollNumber)
	{
		cancelTyping = false;
		isTyping = false;
		hasWaited = false;
		fadeIn = true;
		showText = false;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		cs.SetMoveCamera (false);
		ms.SetMovePlayer (false);
		scrollPanel.SetActive (true);
		Rigidbody rb = player.GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		StartCoroutine (TextScroll ((string)storyLine [scrollNumber]));
	}

	public void CloseStoryMode()
    {
		scrollPanel.SetActive (false);
		pressText.SetActive (false);
		closeBtn.SetActive (false);
		cs.SetMoveCamera (true);
		ms.SetMovePlayer (true);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		cancelTyping = false;
		isTyping = false;
		hasWaited = false;
		fadeIn = false;
		showText = false;

		scrollImage.color = initColor;

		numberScrollsFound += 1;
	}

	public static void SetNumberScrollsFound(int s)
	{
		numberScrollsFound = s;
	}

	public static int GetNumberScrollsFound()
	{
		return numberScrollsFound;
	}
}
