using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SpeechBubble : MonoBehaviour
{
    public GameObject speechBubbleUIPrefab;
    public Vector2 speechBubbleStartOffset = new Vector2(0f, 0f);
    public Vector2 speechBubbleEndOffset = new Vector2(0f, 1f);
    public Vector2 speechBubbleStartScale = new Vector2(0f, 0f);
    public Vector2 speechBubbleEndScale = new Vector2(1f, 1f);
    public float openAnimDuration = 0.5f;
    public float closeAnimDuration = 0.5f;
    public float startTypewriterDelay = 0.2f;

    bool opened = false;
    bool closing = false;
    GameObject speechBubbleObject;
    TMP_Text speechBubbleText;
    TypewriterEffect speechBubbleTypewriter;
    Coroutine deleteBubbleCoroutine;
    Coroutine displayTextCoroutine;
    Coroutine closeBubbleCoroutine;

    Tween positionTween;
    Tween scaleTween;

	private void OnDestroy()
	{
		if (speechBubbleObject)
		{
            Destroy(speechBubbleObject);
		}
	}

	void CreateOrResetBubble()
	{
        if (!speechBubbleObject)
		{
            speechBubbleObject = Instantiate(speechBubbleUIPrefab, transform);
            speechBubbleObject.transform.eulerAngles = Vector3.zero;
		}

        speechBubbleObject.transform.localPosition = speechBubbleStartOffset;
        speechBubbleObject.transform.localScale = speechBubbleStartScale;

        speechBubbleText = speechBubbleObject.GetComponentInChildren<TMP_Text>();

        if (!speechBubbleText)
		{
            Debug.LogError("No TMP_Text field found in the speech bubble UI prefab");
            return;
            // speechBubbleText = speechBubbleObject.AddComponent<TMP_Text>();
		}

        speechBubbleText.text = "";

        speechBubbleTypewriter = speechBubbleObject.GetComponentInChildren<TypewriterEffect>();

        if (!speechBubbleTypewriter)
        {
            Debug.LogError("No TypewriterEffect component found in the speech bubble UI prefab");
            // speechBubbleTypewriter = speechBubbleObject.AddComponent<TypewriterEffect>();
        }
    }

    void RemoveBubble()
	{
        Destroy(speechBubbleObject);
        speechBubbleText = null;
        speechBubbleTypewriter = null;
	}

    void ResetTweens()
	{
        if (positionTween != null)
		{
            positionTween.Kill();
		}

        if (scaleTween != null)
		{
            scaleTween.Kill();
		}

        positionTween = null;
        scaleTween = null;
	}

    public void OpenSpeechBubble(string text, float duration, bool keepOpen = false)
	{
        if (opened)
		{
            return;
		}

        if (closing || deleteBubbleCoroutine != null)
		{
            closing = false;
            StopCoroutine(deleteBubbleCoroutine);
            deleteBubbleCoroutine = null;

            ResetTweens();
		}

        opened = true;

        CreateOrResetBubble();

        displayTextCoroutine = StartCoroutine(DisplayText(startTypewriterDelay, text));

        positionTween = DOTween.To(() => speechBubbleObject.transform.localPosition, x => speechBubbleObject.transform.localPosition = x, (Vector3)speechBubbleEndOffset, openAnimDuration);
        scaleTween = DOTween.To(() => speechBubbleObject.transform.localScale, x => speechBubbleObject.transform.localScale = x, (Vector3)speechBubbleEndScale, openAnimDuration);

        if (!keepOpen)
		{
            closeBubbleCoroutine = StartCoroutine(CloseBubble(duration));
		}
	}

    public void CloseSpeechBubble()
	{
        if (!opened || closing)
		{
            return;
		}

        if (closeBubbleCoroutine != null)
		{
            StopCoroutine(closeBubbleCoroutine);
            closeBubbleCoroutine = null;
		}

        if (displayTextCoroutine != null)
		{
            StopCoroutine(displayTextCoroutine);
            displayTextCoroutine = null;

            ResetTweens();
		}

        opened = false;
        closing = true;

        deleteBubbleCoroutine = StartCoroutine(DeleteBubble(closeAnimDuration));

        positionTween = DOTween.To(() => speechBubbleObject.transform.localPosition, x => speechBubbleObject.transform.localPosition = x, (Vector3)speechBubbleStartOffset, openAnimDuration);
        scaleTween = DOTween.To(() => speechBubbleObject.transform.localScale, x => speechBubbleObject.transform.localScale = x, (Vector3)speechBubbleStartScale, openAnimDuration);
    }

    IEnumerator DeleteBubble(float delay)
	{
        yield return new WaitForSeconds(delay);

        RemoveBubble();
        ResetTweens();
        closing = false;
        deleteBubbleCoroutine = null;
	}

    IEnumerator DisplayText(float delay, string text)
	{
        yield return new WaitForSeconds(delay);

        speechBubbleTypewriter.NewText(text);
        displayTextCoroutine = null;
    }

    IEnumerator CloseBubble(float delay)
	{
        yield return new WaitForSeconds(delay);

        CloseSpeechBubble();
        closeBubbleCoroutine = null;
	}
}
