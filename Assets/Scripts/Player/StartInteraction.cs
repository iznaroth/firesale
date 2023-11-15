using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInteraction : MonoBehaviour
{

	private void OnCollisionEnter2D(Collision2D collision)
	{
		PedestrianAI ped = collision.gameObject.GetComponent<PedestrianAI>();

		if (ped != null)
		{
			ped.Freeze();
			DialogueManager.instance.StartDialogueInteraction(collision.gameObject);
		}
	}

}
