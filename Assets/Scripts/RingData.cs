using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RingData : MonoBehaviour {

	public float m_radius = 50;
	public float m_rayMaxError = 0.2f;

	public static float Radius { get; private set; }
	public static float Diameter { get; private set; }
	public static float Circumference { get; private set; }
	public static float RayErrorAngle { get; private set; }
	public static float RayMaxError { get; private set; }
	public static float RayMaxDistance { get; private set; }
	public static float RayArcDistance { get; private set; }

	private void Awake()
	{
		AssignValues();
	}

	private void AssignValues()
	{
		Radius = m_radius;
		RayMaxError = m_rayMaxError;
		Diameter = m_radius * 2;
		Circumference = m_radius * 2 * Mathf.PI;

		RayMaxDistance = Mathf.Sqrt(m_rayMaxError * m_rayMaxError + 2 * m_rayMaxError * m_radius);
		float errorRadians = Mathf.Acos(m_radius / (m_radius + m_rayMaxError));
		RayErrorAngle = errorRadians * Mathf.Rad2Deg;
		RayArcDistance = errorRadians * m_radius;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		m_radius = Mathf.Max(0.001f, m_radius);
		m_rayMaxError = Mathf.Max(0.001f, m_rayMaxError);
		AssignValues();
	}

	private void OnDrawGizmosSelected()
	{
		if (RayMaxDistance <= 0.01f) return;

		Vector3 pointZero = RingObject.RingPositionY(0, 1);

		Color col1 = Color.blue;
		col1.a = 0.4f;
		Color col2 = Color.red;
		Color col3 = Color.red;
		col2.a = 0.4f;

		// Draw nice ring first
		Vector3 lastPos = pointZero;
		Gizmos.color = col1;
		for (float i = 0; i < 360; i += 0.01f) {
			Vector3 pos = RingObject.RingPositionY(i, 1);
			Gizmos.DrawLine(lastPos, pos);
			lastPos = pos;
		}
		Gizmos.DrawLine(lastPos, pointZero);

		// Then draw error ring

		float deg = 0;
		lastPos = pointZero;

		while ((deg += RayErrorAngle) <= 360) {

			Gizmos.color = col2;

			Vector3 forward = -lastPos.normalized.SetY(0);
			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			Vector3 estPos = lastPos + right * RayMaxDistance;
			Vector3 nxtPos = RingObject.RingPositionY(deg, 1);

			Gizmos.DrawLine(lastPos, estPos);
			Gizmos.DrawLine(estPos, nxtPos);
			lastPos = nxtPos;

			Gizmos.color = col2;
			Gizmos.DrawLine(Vector3.zero, lastPos);
		}

		Gizmos.color = col2;
		Gizmos.DrawLine(lastPos, pointZero);
	}
#endif

}
