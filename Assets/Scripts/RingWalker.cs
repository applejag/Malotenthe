using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System;

public class RingWalker : MonoBehaviour {

	public bool FacingRight { get; protected set; }

	private Rigidbody m_Body;
	public Rigidbody Body { get { return m_Body; } }

	protected virtual void Awake() {
		m_Body = GetComponent<Rigidbody>();
	}

	protected virtual void FixedUpdate()
	{
		if (Body) {
			Vector3 position = Body.position;

			// Set position but with interpolation
			Body.MovePosition(RingPosition(position));

			// Look towards the center
			Body.MoveRotation(RingRotation(position));
		}
	}

	/// <summary>
	/// Returns a new position from <paramref name="degrees"/> that's on the ring
	/// </summary>
	public static Vector3 RingPosition(float degrees, float y = 0)
	{
		return degrees.FromDegrees(RingData.Radius).x_y(_: y);
	}

	/// <summary>
	/// Returns a new position from <paramref name="degrees"/> that's on a custom ring <paramref name="radius"/>
	/// </summary>
	public static Vector3 RingPosition(float degrees, float radius, float y = 0)
	{
		return degrees.FromDegrees(radius).x_y(_: y);
	}

	/// <summary>
	/// Returns a new position from <paramref name="position"/> that's on the ring
	/// </summary>
	public static Vector3 RingPosition(Vector3 position)
	{
		return (position.xz().normalized * RingData.Radius).x_y(_: position.y);
	}

	/// <summary>
	/// Returns a new position from <paramref name="position"/> that's on a custom ring <paramref name="radius"/>
	/// </summary>
	public static Vector3 RingPosition(Vector3 position, float radius)
	{
		return (position.xz().normalized * radius).x_y(_: position.y);
	}

	/// <summary>
	/// Returns a rotation from <paramref name="position"/> facing in towards the center of the ring.
	/// </summary>
	public static Quaternion RingRotation(Vector3 position)
	{
		return Quaternion.LookRotation(-position.SetY(0));
	}

	/// <summary>
	/// Get the degree value of the <paramref name="position"/>
	/// </summary>
	public static float RingDegrees(Vector3 position)
	{
		return position.xz().ToDegrees();
	}

	/// <summary>
	/// Projects the <paramref name="direction"/> vector so it aligns with the ring.
	/// </summary>
	public static Vector3 RingProjectDirection(Vector3 position, Vector3 direction)
	{
		Vector3 normal = (-position.xz().normalized).x_y(0);
		return Vector3.ProjectOnPlane(direction, normal);
	}

	public static bool RingRaycast(Vector3 position, Vector3 forward, out RingRaycastHit hit, float maxDistance, int layerMask)
	{
		float radius = position.magnitude;
		forward = RingProjectDirection(position, forward).normalized;

		List<Vector3> origins = new List<Vector3> { position };
		RaycastHit rayhit;

		while (maxDistance > RingData.RayMaxDistance) {
			Debug.DrawRay(position, forward.normalized * RingData.RayMaxDistance, Color.cyan, 2);

			if (Physics.Raycast(position, forward, out rayhit, RingData.RayMaxDistance, layerMask)) {
				hit = new RingRaycastHit {
					hit = rayhit,
					origins = origins.ToArray(),
					lastPoint = rayhit.point,
				};
				return true;
			}

			maxDistance -= RingData.RayArcDistance;
			position = RingPosition(position + forward * RingData.RayArcDistance);
			forward = RingProjectDirection(position, forward).normalized;
			origins.Add(position);
		}

		Debug.DrawRay(position, forward.normalized * maxDistance, Color.cyan, 2);
		bool success = Physics.Raycast(position, forward, out rayhit, maxDistance, layerMask);

		hit = new RingRaycastHit {
			hit = rayhit,
			origins = origins.ToArray(),
			lastPoint = success ? rayhit.point : RingPosition(position + forward * RingData.RayArcDistance),
		};

		return success;
	}

}

public struct RingRaycastHit
{
	public Vector3[] origins;
	public Vector3 lastPoint;
	public RaycastHit hit;
}
