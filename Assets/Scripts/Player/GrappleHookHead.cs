using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHookHead : MonoBehaviour
{
	private void Awake()
	{
		transform.parent = null;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		HandleCollision(collision.collider);
	}

	void HandleCollision(Collider2D collider)
	{
		PedestrianAI ped = collider.GetComponent<PedestrianAI>();

		if (ped == null)
		{
			GrappleHookController.instance?.RetractHookEmpty();
		}
		else
		{
			GrappleHookController.instance?.OnGrappleHit?.Invoke(ped);
		}
	}
}
