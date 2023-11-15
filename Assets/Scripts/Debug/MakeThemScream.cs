using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeThemScream : MonoBehaviour
{
	public int minimumLength = 5;
	public int maximumLength = 50;
	public string availableChars = "AAAAAAGH ";

	public float minDuration = 3f;
	public float maxDuration = 8f;

	string ConstructScream()
	{
		int length = Random.Range(minimumLength, maximumLength);
		string result = "";

		for (int i = 0; i < length; i++)
		{
			result += availableChars[Random.Range(0, availableChars.Length)];
		}

		return result;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		SpeechBubble speech = collision.collider.gameObject.GetComponent<SpeechBubble>();
		if (speech != null)
		{
			speech.OpenSpeechBubble(ConstructScream(), Random.Range(minDuration, maxDuration));
		}
	}
}
