using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class MeteoriteSpawner : RingObject
{
	public ParticleSystem particles;
	public Animator anim;
	public GameObject prefab;
	public float selfDestructAfter = 2;
	public LayerMask hitMask = 1;
	[Range(0, 2)] public float gravityScale = 1;
	public float initialSpeed = 25;

	[SerializeField, HideInInspector] private Vector3 velocity;
	[SerializeField, HideInInspector] private bool grounded;

	private void Start()
	{
		velocity = Random.Range(0, 360).FromDegrees(initialSpeed).x_y(0);
	}

	private void OnValidate()
	{
		if (selfDestructAfter < 0)
			selfDestructAfter = -1;
	}

	private void FixedUpdate()
	{
		if (grounded) return;

		RingRaycastHit hit;
		float maxDistance = velocity.magnitude * Time.fixedDeltaTime;
		velocity += Physics.gravity * Time.fixedDeltaTime;
		Vector3 forward = transform.forward * maxDistance + velocity * Time.fixedDeltaTime;

		bool hitAnything = RingRaycast(transform.position, forward, out hit, maxDistance, hitMask);

		Vector3 lastPosition = transform.position;
		transform.position = hit.lastPoint;
		transform.forward = transform.position - lastPosition;

		if (hitAnything)
		{
			grounded = true;
			transform.rotation = RingRotation(transform.position);
			anim.SetBool("Grounded", true);
			particles.Play();
			Destroy(gameObject, selfDestructAfter);
		}
	}

	public void SpawnPrefab()
	{
		if (!prefab) return;
		Instantiate(prefab, transform.position, transform.rotation);
	}
}
