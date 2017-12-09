using UnityEngine;
using UnityEditor;
using System.Collections;
using ExtensionMethods;

[CustomEditor(typeof(RingData))]
public class e_RingData : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Static values", EditorStyles.boldLabel);
		bool oldEnabled = GUI.enabled;
		GUI.enabled = false;
		EditorGUILayout.FloatField("Radius", RingData.Radius);
		EditorGUILayout.FloatField("Diameter", RingData.Diameter);
		EditorGUILayout.FloatField("Circumference", RingData.Circumference);
		EditorGUILayout.FloatField("Ray Error Angle", RingData.RayErrorAngle);
		EditorGUILayout.FloatField("Ray Max Error", RingData.RayMaxError);
		EditorGUILayout.FloatField("Ray Max Distance", RingData.RayMaxDistance);
		EditorGUILayout.FloatField("Ray Arc Distance", RingData.RayArcDistance);
		GUI.enabled = oldEnabled;

		EditorGUILayout.Space();

		if (GUILayout.Button("Send test ray")) {
			Vector3 position = RingWalker.RingPositionY(degrees: 0, y: 2);
			Vector3 forward = -position.SetY(0).normalized;
			Vector3 right = new Vector3(forward.z, 0, -forward.x);

			RingRaycastHit hit;
			RingWalker.RingRaycast(position, right + Vector3.up * 0.01f, out hit, RingData.Circumference * 4f, 0);
		}

		EditorGUILayout.Space();
	}

}
