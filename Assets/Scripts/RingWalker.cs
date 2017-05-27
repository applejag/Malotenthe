using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public abstract class RingWalker : MonoBehaviour {
	
	public float RingDegrees {
		get {
			return transform.position.xz().ToDegrees();
		}
		set {
			JumpToPosition(value);
		}
	}

	public float RingRadius {
		get {
			return (body ? body.position : transform.position).magnitude;
		}
		set {
			if (body)
				body.MovePosition(body.position.normalized * value);
			else
				transform.position = transform.position.normalized * value;
		}
	}

	public bool facingRight { get; protected set; }

	public Rigidbody body { get; private set; }

	protected virtual void Awake() {
		body = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Move <seealso cref="RingWalker"/> to position specified via ring degrees.
	/// </summary>
	public void JumpToPosition(float degrees) {
		if (body) {
			var pos = body.position;
			var radius = pos.magnitude;
			body.position = degrees.FromDegrees(radius).x_y(_: pos.y);
		} else {
			var pos = transform.position;
			var radius = pos.magnitude;
			transform.position = degrees.FromDegrees(radius).x_y(_: pos.y);
		}
	}

}
