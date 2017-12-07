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

		GUI.enabled = script.worldObject && script.worldObject.gameObject.scene.IsValid();
		
		if (GUILayout.Button("Jump to target now"))
		{
			script.JumpToTarget();
		}
		GUI.enabled = true;
	}

}
