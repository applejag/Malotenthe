using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using ExtensionMethods;

[CustomEditor(typeof(WalkerStatistics))]
[CanEditMultipleObjects]
public class e_WalkerStatistics : Editor
{
	public override void OnInspectorGUI() {

		EditorGUILayout.Space();

		WalkerStatistics[] stats = targets.Cast<WalkerStatistics>().ToArray();

		string label = stats.Length == 1
			? (stats[0].Walker != null
				? string.Format("Statistics for {0} ({1})", stats[0].name, stats[0].Walker.GetType().Name)
				: string.Format("Statistics for {0} (Missing RingWalker component!)", stats[0].name))
			: string.Format("Statistics for {0} walkers", stats.Length);

		EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
		bool oldEnabled = GUI.enabled;
		GUI.enabled = false;
		EditorGUILayout.IntField("Damage taken", stats.Sum(s => s.DamageTaken));
		EditorGUILayout.IntField("Damage dealt", stats.Sum(s => s.DamageDealt));
		EditorGUILayout.IntField("Number of kills", stats.Sum(s => s.NumOfKills));
		EditorGUILayout.FloatField("Time alive", stats.Sum(s => s.TimeAliveTotal));

		GUI.enabled = oldEnabled;
	}

}
