using UnityEngine;
using UnityEditor;
using System.Collections;
using GameGUI;

[CustomEditor(typeof(AnchoredFollow))]
public class e_GameGUI_AnchoredFollow : Editor
{

	private AnchoredFollow script;

	private void OnEnable()
	{
		script = target as AnchoredFollow;
	}

	public override void OnInspectorGUI() {

		DrawDefaultInspector();
		
		EditorGUILayout.Space();
		GUI.enabled = false;
		EditorGUILayout.Vector3Field("Used Position", script.usedPos);
		GUI.enabled = true;
		if (GUILayout.Button("Jump to target now"))
		{
			script.JumpToTarget();
		}
	}

}
