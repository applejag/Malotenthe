using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
public class e_Transform : Editor {
	
	private bool move;
	private Vector2 mouse;
	private RaycastHit lastHit;

	private Material mat;
	private Material mat2;

	private string Label { get { return Selection.transforms.Length > 1 ? "Move (" + Selection.transforms.Length + ") objects" : "Move " + Selection.transforms[0].name; } }

	void OnEnable() {
		this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
		this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
		this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");


		mat = new Material(Shader.Find("Sprites/Default"));
		mat.color = new Color(0.5f, 0.5f, 0f, 0.5f);

		mat2 = new Material(Shader.Find("Sprites/Default"));
		mat2.color = new Color(1f, 0f, 0f, 0.1f);
	}

	void OnDisable() {
		DestroyImmediate(mat);
		DestroyImmediate(mat2);
	}

	void OnSceneGUI() {
		if (!move) return;

		Event e = Event.current;

		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		switch (e.GetTypeForControl(controlID)) {
			case EventType.MouseDown:
				if (e.button == 1) {
					move = false;
					GUIUtility.hotControl = 0;
					e.Use();
				} else if (e.button == 0) {
					GUIUtility.hotControl = controlID;
					e.Use();
				}
				break;

			case EventType.MouseUp:
				if (e.button != 0) break;

				// Calculate center
				Vector3[] all = new Vector3[Selection.transforms.Length];
				for (int i = 0; i < Selection.transforms.Length; i++) {
					all[i] = Selection.transforms[i].position;
				}
				Vector3 center = VectorHelper.Average(all);

				// Move em
				foreach (var t in Selection.transforms) {
					Undo.RecordObject(t, "Move position");
					Vector3 newpos = lastHit.point + t.position - center;
					if (e.control)
						newpos = newpos.Round();
					t.position = newpos;
				}
				
				move = false;
				GUIUtility.hotControl = 0;
				e.Use();

				Repaint();
				break;

			case EventType.MouseMove:
				mouse = Event.current.mousePosition;
				HandleUtility.Repaint();
				break;

			case EventType.KeyDown:
				if (e.keyCode == KeyCode.Escape && move) {
					// Abort
					move = false;
					GUIUtility.hotControl = 0;
					e.Use();
				}
				break;

		}

		if (Camera.current && e.type == EventType.Repaint) {
			Ray ray = HandleUtility.GUIPointToWorldRay(mouse);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Camera.current.farClipPlane, LayerMask.GetMask("Default", "Terrain"))) {
				Handles.color = new Color(0, 1, 1);
				Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
				Handles.ArrowHandleCap(controlID, hit.point, rotation, 1, EventType.Ignore);
				Handles.CircleHandleCap(controlID, hit.point, rotation, 1, EventType.Ignore);

				#region Draw meshes
				foreach (var filter in hit.collider.GetComponentsInChildren<MeshFilter>()) {
					mat2.SetPass(0);
					Handles.color = Color.magenta;
					Graphics.DrawMeshNow(filter.sharedMesh, Matrix4x4.TRS(filter.transform.position, filter.transform.rotation, filter.transform.lossyScale));
				}
				#endregion

				#region Draw box colliders
				foreach (var box in hit.collider.GetComponentsInChildren<BoxCollider>()) {
					Handles.color = Color.red;

					Vector3 pos = box.transform.position;
					Vector3 half = box.size / 2;

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, half.z)));


					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, -half.z)));


					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, half.z)));
				}
				#endregion

				#region Draw what's held

				// Calculate center
				Vector3[] all = new Vector3[Selection.transforms.Length];
				for (int i = 0; i < Selection.transforms.Length; i++) {
					all[i] = Selection.transforms[i].position;
				}
				Vector3 center = VectorHelper.Average(all);

				Transform t = target as Transform;
				Vector3 newpos = lastHit.point + t.position - center;
				if (e.control)
					newpos = newpos.Round();

				Handles.color = new Color(1, 1, 0, .5f);
				Handles.CircleHandleCap(controlID, newpos, rotation, 1, EventType.Ignore);
				foreach (var filter in t.GetComponentsInChildren<MeshFilter>()) {
					Vector3 offset = filter.transform.position - t.position;
					mat.SetPass(0);
					Graphics.DrawMeshNow(filter.sharedMesh, Matrix4x4.TRS(newpos + offset, filter.transform.rotation, filter.transform.lossyScale));
				}

				#endregion

				Handles.Label(newpos + Vector3.up * HandleUtility.GetHandleSize(newpos)/2, Label+"\nLMB to move\nRMB to abort");
				lastHit = hit;
			}
		}
	}

	public override void OnInspectorGUI() {

		EditorGUIUtility.wideMode = WIDE_MODE;
		EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - FIELD_WIDTH; // align field to right of inspector

		this.serializedObject.Update();

		EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
		this.RotationPropertyField(this.rotationProperty, rotationGUIContent);
		EditorGUILayout.PropertyField(this.scaleProperty, scaleGUIContent);

		if (!ValidatePosition(((Transform)this.target).position)) {
			EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
		}

		if (Selection.transforms.Length > 0) {
			GUI.enabled = !move;
			if (GUILayout.Button(Label)) {
				move = true;
			}
			GUI.enabled = true;
		}

		this.serializedObject.ApplyModifiedProperties();
	}

	#region TransformInspector.cs
	private const float FIELD_WIDTH = 212.0f;
	private const bool WIDE_MODE = true;

	private const float POSITION_MAX = 100000.0f;

	private static readonly GUIContent positionGUIContent = new GUIContent(LocalString("Position")
																 , LocalString("The local position of this Game Object relative to the parent."));
	private static readonly GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation")
																 , LocalString("The local rotation of this Game Object relative to the parent."));
	private static readonly GUIContent scaleGUIContent = new GUIContent(LocalString("Scale")
																 , LocalString("The local scaling of this Game Object relative to the parent."));

	private static readonly string positionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

	private SerializedProperty positionProperty;
	private SerializedProperty rotationProperty;
	private SerializedProperty scaleProperty;

	private static string LocalString(string text) {
		// LocalizationDatabase deprecated or somethin?
		//return LocalizationDatabase.GetLocalizedString(text);
		return text;
	}

	private bool ValidatePosition(Vector3 position) {
		if (Mathf.Abs(position.x) > POSITION_MAX) return false;
		if (Mathf.Abs(position.y) > POSITION_MAX) return false;
		if (Mathf.Abs(position.z) > POSITION_MAX) return false;
		return true;
	}

	private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content) {
		Transform transform = (Transform)this.targets[0];
		Quaternion localRotation = transform.localRotation;
		foreach (UnityEngine.Object t in (UnityEngine.Object[])this.targets) {
			if (!SameRotation(localRotation, ((Transform)t).localRotation)) {
				EditorGUI.showMixedValue = true;
				break;
			}
		}

		EditorGUI.BeginChangeCheck();

		Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObjects(this.targets, "Rotation Changed");
			foreach (UnityEngine.Object obj in this.targets) {
				Transform t = (Transform)obj;
				t.localEulerAngles = eulerAngles;
			}
			rotationProperty.serializedObject.SetIsDifferentCacheDirty();
		}

		EditorGUI.showMixedValue = false;
	}

	private bool SameRotation(Quaternion rot1, Quaternion rot2) {
		if (rot1.x != rot2.x) return false;
		if (rot1.y != rot2.y) return false;
		if (rot1.z != rot2.z) return false;
		if (rot1.w != rot2.w) return false;
		return true;
	}
	#endregion

}
