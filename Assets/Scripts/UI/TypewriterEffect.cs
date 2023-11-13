using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
	TMP_Text _tmpProText;
	string writer;

	[SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float delayAfterEnd = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;

	// Use this for initialization
	void Start()
	{
		DialogueManager.playerCanRespond = false;
		_tmpProText = GetComponent<TMP_Text>()!;

		if (_tmpProText != null)
		{
			_tmpProText.text = "";
		}
	}
    private void Awake()
    {
		_tmpProText = GetComponent<TMP_Text>()!;
	}

    public void NewText(string newText)
    {
		_tmpProText = GetComponent<TMP_Text>()!;
		writer = newText;
		DialogueManager.playerCanRespond = false;
		StartCoroutine("TypeWriterTMP");
	}

	IEnumerator TypeWriterTMP()
	{
		DialogueManager.playerCanRespond = false;
		_tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

		yield return new WaitForSeconds(delayBeforeStart);

		foreach (char c in writer)
		{
			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
			}
			_tmpProText.text += c;
			_tmpProText.text += leadingChar;
			yield return new WaitForSeconds(timeBtwChars);
		}

		if (leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
		}

		yield return new WaitForSeconds(delayAfterEnd);
		DialogueManager.playerCanRespond = true;
	}
}
