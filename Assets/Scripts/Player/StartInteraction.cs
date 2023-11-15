using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInteraction : MonoBehaviour
{
	List<GameObject> interactedWith = new List<GameObject>();


	private void OnCollisionEnter2D(Collision2D collision)
	{
		PedestrianAI ped = collision.gameObject.GetComponent<PedestrianAI>();

		if (ped == null)
		{
			return;
		}

		if (interactedWith.Contains(collision.gameObject))
		{
			return;
		}

		ped.Freeze();
		DialogueManager.instance.StartDialogueInteraction(collision.gameObject);

		interactedWith.Add(collision.gameObject);
		ped.OnRemove += () => { interactedWith.Remove(ped.gameObject); };
	}

}
