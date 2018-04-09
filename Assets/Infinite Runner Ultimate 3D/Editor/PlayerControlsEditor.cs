using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor (typeof(PlayerControls))]
public class PlayerControlsEditor : Editor {
	private SerializedObject pcobject;
	private PlayerControls pc;
	void OnEnable () {
		pcobject= new SerializedObject(target);
		pc=(PlayerControls)target;
		
		
	}
	public override void OnInspectorGUI () {
		pcobject.Update();
		EditorGUILayout.BeginVertical();
		DrawDefaultInspector();
		pc.UseEnemy=EditorGUILayout.Toggle("Use Enemy",pc.UseEnemy);
		if(pc.UseEnemy){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			pc.Enemy=(GameObject)EditorGUILayout.ObjectField("Enemy Game Object", pc.Enemy, typeof(GameObject), true);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			pc.enemyPosition=EditorGUILayout.Vector3Field("Enemy Position",pc.enemyPosition);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			pc.enemybackPosition=EditorGUILayout.Vector3Field("Enemy Back Position",pc.enemybackPosition);
			EditorGUILayout.EndHorizontal();
		}
		GUILayout.Label("Player Speeds");
		if(pc.SpeedDist.Count==0){
			pc.SpeedDist.Add(new SpeedandDistance());
		}
		for(int i=0; i<pc.SpeedDist.Count;++i){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			GUILayout.Label("Move with Speed");
			pc.SpeedDist[i].Speed=EditorGUILayout.FloatField(pc.SpeedDist[i].Speed);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUILayout.Label("Till Distance");
			if(i==pc.SpeedDist.Count-1){
				pc.SpeedDist[i].Distance=0f;
				GUILayout.Label("Infinity");
			}else
			pc.SpeedDist[i].Distance=EditorGUILayout.FloatField(pc.SpeedDist[i].Distance);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Add new Speed")){
			pc.SpeedDist.Add(new SpeedandDistance());
		}
		if(GUILayout.Button("Remove Speed")){
			pc.SpeedDist.RemoveAt(pc.SpeedDist.Count-1);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		if(GUI.changed){
			EditorUtility.SetDirty(target);
			EditorUtility.SetDirty(pc);
			pcobject.ApplyModifiedProperties();
		}

	}

}
