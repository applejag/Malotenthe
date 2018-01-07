using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : RingObject
{
	[Header("Object references")]
	public TrailRenderer trail;
	public ParticleSystem particles;
	[HideInInspector]
	public RingWalker owner = null;

	[Header("Settings")]
	public int damage = 1;
	public float speed = 20;
	public LayerMask hitMask = 1;
	public float selfDestruct = 5;
	[Range(0,2)]
	public float gravityScale = 1;
	
	private Vector3 gravity;
	[SerializeField, HideInInspector]
	private float startTime;

	private void Start()
	{
		startTime = Time.time;
		SelfDestruct(selfDestruct);
	}

	private void FixedUpdate()
	{
		RingRaycastHit hit;
		float maxDistance = speed * Time.fixedDeltaTime;
		gravity += Physics.gravity * Time.fixedDeltaTime * gravityScale;
		Vector3 forward = transform.forward * maxDistance + gravity * Time.fixedDeltaTime;

		bool hitAnything = RingRaycast(transform.position, forward, out hit, maxDistance, hitMask);

		Vector3 lastPosition = transform.position;
		transform.position = hit.point;
		transform.forward = transform.position - lastPosition;

		if (hitAnything) {

			RingWalker walker = hit.rigidbody
				? hit.rigidbody.GetComponent<RingWalker>()
				: null;
			if (walker)
				walker.Damage(damage, this);

			SelfDestruct();
		}
	}

	private void SelfDestruct(float delay)
	{
		Invoke("SelfDestruct", delay);
	}
	
	private void SelfDestruct()
	{
		if (particles) {
			particles.transform.SetParent(null);
			var em = particles.emission;
			em.enabled = false;
			Destroy(particles.gameObject, particles.main.startLifetime.constantMax);
		}

		if (trail) {
			trail.transform.SetParent(null);
			trail.autodestruct = true;

			float lived = Time.time - startTime;
			trail.time = Mathf.Min(lived, trail.time);
		}

		Destroy(gameObject);
	}

}
