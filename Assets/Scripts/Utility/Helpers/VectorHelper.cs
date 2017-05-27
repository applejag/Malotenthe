using UnityEngine;
using System.Collections;

public static class VectorHelper {

	/// <summary>
	/// Get the average from a list of <seealso cref="Vector2"/> vectors.
	/// </summary>
	public static Vector2 Average(params Vector2[] vectors) {
		if (vectors.Length == 0)
			return Vector2.zero;

		Vector2 total = Vector2.zero;
		foreach (var vec in vectors) {
			total += vec;
		}

		return total / vectors.Length;
	}

	/// <summary>
	/// Get the average from a list of <seealso cref="Vector3"/> vectors.
	/// </summary>
	public static Vector3 Average(params Vector3[] vectors) {
		if (vectors.Length == 0)
			return Vector3.zero;

		Vector3 total = Vector3.zero;
		foreach (var vec in vectors) {
			total += vec;
		}

		return total / vectors.Length;
	}

	/// <summary>
	/// Calculates the velocity needed to send a <seealso cref="Rigidbody"/> to the vector /<paramref name="vector"/>/ in a specific amount of time.
	/// </summary>
	/// <param name="vector">The offset <seealso cref="Vector3"/> that the <seealso cref="Rigidbody"/> shall reach.</param>
	/// <param name="timesteps">Number of physics timesteps (see <see cref="Time.fixedDeltaTime"/>) the <seealso cref="Rigidbody"/> should take 'til it reaches its destination.</param>
	public static Vector3 CalculateVelocity(Vector3 vector, float timesteps) {

		Vector3 acceleration = Time.fixedDeltaTime * Time.fixedDeltaTime * Physics.gravity;

		// http://morgan-davidson.com/2012/06/19/3d-projectile-trajectory-prediction/
		Vector3 velocity = -((acceleration * (timesteps*timesteps + timesteps)) * 0.5f - vector) / timesteps;

		// Since PhysX velocity is based of units/second not units/timestep
		return velocity /= Time.fixedDeltaTime;
	}

	/// <summary>
	/// Returns the absolute value of every component of the vector.
	/// </summary>
	public static Vector2 Abs(this Vector2 vector) {
		return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
	}

	/// <summary>
	/// Returns the absolute value of every component of the vector.
	/// </summary>
	public static Vector3 Abs(this Vector3 vector) {
		return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
	}

	/// <summary>
	/// Returns a vector where every component has been rounded
	/// </summary>
	public static Vector2 Round(this Vector2 vector) {
		vector.x = Mathf.Round(vector.x);
		vector.y = Mathf.Round(vector.y);
		return vector;
	}

	/// <summary>
	/// Returns a vector where every component has been rounded
	/// </summary>
	public static Vector3 Round(this Vector3 vector) {
		vector.x = Mathf.Round(vector.x);
		vector.y = Mathf.Round(vector.y);
		vector.z = Mathf.Round(vector.z);
		return vector;
	}

}
