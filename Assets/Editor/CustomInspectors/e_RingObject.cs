using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using ExtensionMethods;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

[CustomEditor(typeof(RingObject), true)]
[CanEditMultipleObjects]
public class e_RingObject : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		{
			float? allDist = null;
			foreach (var o in targets)
			{
				var t = (RingObject)o;
				float dist = Mathf.Abs(t.transform.position.xz().magnitude - RingData.Radius);
				if (!allDist.HasValue)
					allDist = dist;
				else if (Math.Abs(allDist.Value - dist) > 0.001f)
				{
					allDist = null;
					break;
				}
			}
			GUI.enabled = false;
			EditorGUI.showMixedValue = !allDist.HasValue;
			EditorGUILayout.FloatField("Distance from ring", allDist ?? 0);
			EditorGUI.showMixedValue = false;
			EditorGUILayout.Separator();
			GUI.enabled = true;
		}

		if (GUILayout.Button("Jump to ring")) {
			float time = 2;

			Color col1 = Color.blue;
			col1.a = 0.4f;
			
			Undo.RecordObjects(targets, "Move objects to ring");
			foreach (var t in targets) {
				RingObject script = t as RingObject;
				Vector3 newpos = RingObject.RingPosition(script.transform.position);
				script.transform.position = newpos;
				script.transform.rotation = RingObject.RingRotation(script.transform.position);

				float y = script.transform.position.y;
				Vector3 pointZero = RingObject.RingPosition(0, y: y);

				// Draw nice ring first
				Vector3 lastPos = pointZero;
				for (float i = 0; i < 360; i += 0.01f) {
					Vector3 pos = RingObject.RingPosition(i, y: y);
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
