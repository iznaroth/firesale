using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
	TMP_Text _tmpProText;
	string writer;

	[SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float delayAfterEnd = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] float timeBtwWords = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;
	[SerializeField] bool skippable = true;

	// Use this for initialization
	void Start()
	{
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
		_tmpProText.text = "";
		DialogueManager.newDialogueStarted = true;
		Keyboard.current.onTextInput += SkipText;
		StartCoroutine("TypeWriterTMP");
	}

	public void SkipText(char ch)
    {
		if (skippable)
		{
			StopCoroutine("TypeWriterTMP");
			Keyboard.current.onTextInput -= SkipText;
			_tmpProText.text = writer;
			DialogueManager.newDialogueStarted = false;
		}
	}

	IEnumerator TypeWriterTMP()
	{
		DialogueManager.newDialogueStarted = true;
		_tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";
		string styleMarker = "";
		bool changingStyle = false;

		yield return new WaitForSeconds(delayBeforeStart);

		foreach (char c in writer)
		{
			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
			}
			if(c == '<')
            {
				styleMarker += c;
				changingStyle = true;
            }
			else if (c == '>')
            {
				changingStyle = false;
				styleMarker += c;
				_tmpProText.text += styleMarker;
				styleMarker = "";
			}
			
            else if (!changingStyle)
            {
				_tmpProText.text += c;
				_tmpProText.text += leadingChar;
				if(c != ' ')
                {
					yield return new WaitForSeconds(timeBtwChars);
				}
                else
                {
					yield return new WaitForSeconds(timeBtwWords);
				}

			}
            else
            {
				styleMarker += c;
			}
		}

		if (leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
		}

		yield return new WaitForSeconds(delayAfterEnd);
		DialogueManager.newDialogueStarted = false;
		Keyboard.current.onTextInput -= SkipText;
	}
}
