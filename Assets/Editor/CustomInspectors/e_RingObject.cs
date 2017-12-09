using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Object = UnityEngine.Object;

[CustomEditor(typeof(RingObject), true)]
[CanEditMultipleObjects]
public class e_RingObject : Editor {

	private static void CustomFloatField<T>(string label, IEnumerable<T> targets, Func<T, float> selector)
	{
		float? value = null;
		foreach (var o in targets)
		{
			float one = selector(o);

			if (!value.HasValue)
				value = one;
			else if (Math.Abs(value.Value - one) > 0.001f)
			{
				value = null;
				break;
			}
		}
		EditorGUI.showMixedValue = !value.HasValue;
		EditorGUILayout.FloatField(label, value ?? 0);
		EditorGUI.showMixedValue = false;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		RingObject[] ringTargets = targets.OfType<RingObject>().ToArray();

		EditorGUILayout.Space();

		{
			GUI.enabled = false;
			CustomFloatField("Distance from ring", ringTargets, r => Mathf.Abs(r.transform.position.xz().magnitude - RingData.Radius));
			CustomFloatField("Ring degrees", ringTargets, r => RingObject.RingDegrees(r.transform.position));
			EditorGUILayout.Separator();
			GUI.enabled = true;
		}

		if (GUILayout.Button("Jump to ring")) {
			const float time = 2;

			Color col1 = Color.blue;
			col1.a = 0.4f;
			
			Undo.RecordObjects(targets, "Move objects to ring");
			foreach (RingObject script in ringTargets) {
				Vector3 newpos = RingObject.RingPosition(script.transform.position);
				script.transform.position = newpos;
				script.transform.rotation = RingObject.RingRotation(script.transform.position);

				float y = script.transform.position.y;
				Vector3 pointZero = RingObject.RingPositionY(0, y: y);

				// Draw nice ring first
				Vector3 lastPos = pointZero;
				for (float i = 0; i < 360; i += 0.01f) {
					Vector3 pos = RingObject.RingPositionY(i, y: y);
					Debug.DrawLine(lastPos, pos, col1, time);
					lastPos = pos;
				}
				Debug.DrawLine(lastPos, pointZero, col1, time);
				Debug.DrawLine(newpos, newpos.SetY(0), col1, time);
			}

		}
		
		EditorGUILayout.Space();
	}

}
