using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInteraction : MonoBehaviour
{
	List<GameObject> interactedWith = new List<GameObject>();

	bool targetSpecificPed = false;
	GameObject pedToTarget;

	public void TargetSpecificPed(GameObject ped)
	{
		PedestrianAI pedAI = ped.GetComponent<PedestrianAI>();

		if (pedAI != null)
		{
			pedAI.OnRemove += () =>
			{
				if (pedToTarget == ped)
				{
					pedToTarget = null;
					targetSpecificPed = false;
				}
			};
		}

		targetSpecificPed = true;
		pedToTarget = ped;
	}

	public void ResetTargetSpecificPed()
	{
		targetSpecificPed = false;
		pedToTarget = null;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{

		// don't start interaction unless we're holding an item
		if (!GameManager.Player.GetComponent<PlayerController>().IsHolding())
		{
			return;
		}

		if (targetSpecificPed)
		{
			// if we've hit the ped we're targeting, we can forget about it
			if (pedToTarget == collision.gameObject)
			{
				targetSpecificPed = false;
				pedToTarget = null;
			}
			else if (pedToTarget == null)
			{
				// the ped we were targetting probably got deleted and we didn't notice
				targetSpecificPed = false;
			}
			else
			{
				return;
			}
		}


		PedestrianAI ped = collision.gameObject.GetComponent<PedestrianAI>();

		if (ped == null)
		{
			return;
		}

		if (DialogueManager.IsInDialogue())
		{
			return;
		}

		if (interactedWith.Contains(collision.gameObject))
		{
			return;
		}

		ped.Freeze();
		GameManager.Player.GetComponent<PlayerController>()?.Freeze(); // freeze if we're the player
		DialogueManager.instance.StartDialogueInteraction(collision.gameObject, Start_Conditions.Normal);

		interactedWith.Add(collision.gameObject);
		ped.OnRemove += () => { interactedWith.Remove(ped.gameObject); };
	}

}
