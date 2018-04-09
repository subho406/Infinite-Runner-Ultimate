using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
#if !(UNITY_5_0||UNITY_5_1||UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor (typeof(PropPoolManager))]
public class PropPoolManagerEditor : Editor {
	private SerializedObject poolmanagerobject;
	private PropPoolManager poolmanager;
	void OnEnable () {
		poolmanagerobject= new SerializedObject(target);
		poolmanager=(PropPoolManager)target;
		
		
	}
	public override void OnInspectorGUI () {
		poolmanagerobject.Update();
		EditorGUILayout.BeginVertical();

		int i=0;
		foreach( poolObstacle ob in poolmanager.pObstacle){
			poolmanager.foldout[i] = EditorGUILayout.Foldout(poolmanager.foldout[i],"Obstacle "+i);
			if(poolmanager.foldout[i]){
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Name ");
				poolmanager.Names[i]=EditorGUILayout.TextField(poolmanager.Names[i]);
				EditorGUILayout.EndHorizontal();
				ob.obstacle=(GameObject)EditorGUILayout.ObjectField("Obstacle", ob.obstacle, typeof(GameObject), true);
				ob.pooledAmount=EditorGUILayout.FloatField("Pooled Amount",ob.pooledAmount);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Remove Obstacle")){
					poolmanager.pObstacle.RemoveAt(i);
					poolmanager.foldout.RemoveAt(i);
					poolmanager.Names.RemoveAt(i);
					break;
				}
				EditorGUILayout.EndHorizontal();
			}
			++i;
		}

		if (GUILayout.Button("Add new Obstacle")) {
			poolmanager.pObstacle.Add(new poolObstacle());
			poolmanager.foldout.Add(false);

			poolmanager.Names.Add("New Obstacle"+poolmanager.pObstacle.Count.ToString());
		}
		EditorGUILayout.EndVertical();
		if(GUI.changed){
			EditorUtility.SetDirty(target);
			EditorUtility.SetDirty(poolmanager);
            #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
		}
		
		poolmanagerobject.ApplyModifiedProperties();
	
	}
}
