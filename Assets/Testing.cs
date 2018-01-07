using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : RingObject
{
	public float length = 1;
	public LayerMask layerMask = 1;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		RingRaycastHit hit;
		if (RingRaycast(transform.position, transform.forward, out hit, length, layerMask))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(hit.point, 0.2f);
		}
		
		Vector3 last = transform.position;
		foreach (Vector3 next in hit.prevPoints)
		{
			Gizmos.DrawLine(last, next);
			last = next;
		}
		Gizmos.DrawLine(last, hit.point);

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position, transform.forward * length);
	}
}
