using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RingObject : MonoBehaviour
{


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

	/// <summary>
	/// Raycasts on the circle. Makes multiple rays if the ray is too long and bends it around the ring.
	/// </summary>
	public static bool RingRaycast(Vector3 position, Vector3 forward, out RingRaycastHit hit, float maxDistance, int layerMask)
	{
		return RingRaycast(position, forward, out hit, maxDistance, layerMask, RingData.Radius);
	}

	/// <summary>
	/// Raycasts on a custom circle. Makes multiple rays if the ray is too long and bends it around the ring.
	/// </summary>
	public static bool RingRaycast(Vector3 position, Vector3 forward, out RingRaycastHit hit, float maxDistance, int layerMask, float radius)
	{
		bool success = false;
		forward = RingProjectDirection(position, forward).normalized;

		List<Vector3> origins = new List<Vector3> { position };
		RaycastHit rayhit;

		// Too long ray, split it up
		while (maxDistance > RingData.RayMaxDistance) {

			if (success = Physics.Raycast(position, forward, out rayhit, RingData.RayMaxDistance, layerMask)) {
				goto FinishedCasting;
			} else
				Debug.DrawRay(position, forward.normalized * maxDistance, Color.cyan, 2);

			maxDistance -= RingData.RayArcDistance;
			position = RingPosition(position + forward * RingData.RayMaxDistance, radius);
			forward = RingProjectDirection(position, forward).normalized;
			origins.Add(position);
		}

		// One last ray
		success = Physics.Raycast(position, forward, out rayhit, maxDistance, layerMask);

		FinishedCasting:

		if (success)
			Debug.DrawLine(position, rayhit.point, Color.red, 2);
		else
			Debug.DrawRay(position, forward.normalized * maxDistance, Color.cyan, 2);

		// Custom struct
		hit = new RingRaycastHit {
			hit = rayhit,
			origins = origins.ToArray(),
			lastPoint = success ? rayhit.point : RingPosition(position + forward * maxDistance, radius),
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