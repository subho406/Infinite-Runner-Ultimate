
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor (typeof(WorldManager))]
public class WorldManagerEditor : Editor{


	private SerializedObject worldmanagerobject;
	private WorldManager worldmanager;
   
	void OnEnable () {
		worldmanagerobject = new SerializedObject(target);
		worldmanager=(WorldManager)target;


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void OnInspectorGUI () {
		worldmanagerobject.Update();
		EditorGUILayout.BeginVertical();
		int i=0;
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Pool Amount -");
		worldmanager.PoolAmount=EditorGUILayout.IntField(worldmanager.PoolAmount);
		EditorGUILayout.EndHorizontal();
		if(worldmanager.Worlds.Count>0){
		worldmanager.startWorld=EditorGUILayout.IntSlider("Start World ",worldmanager.startWorld,0,worldmanager.Worlds.Count-1);
		}
		worldmanager.StartTarget=(Transform)EditorGUILayout.ObjectField("Player Start Look Target",worldmanager.StartTarget, typeof(Transform), true);
		foreach(World w in worldmanager.Worlds){

			worldmanager.showWorld[i] = EditorGUILayout.Foldout(worldmanager.showWorld[i],"World "+i);
			if(worldmanager.showWorld[i]){
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("World Probability -");

				w.probability = EditorGUILayout.Slider(w.probability,0, 1);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Max Track Length");
				w.maxTracklength=EditorGUILayout.IntSlider(w.maxTracklength,w.minTracklength,50);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label ("Min Track Length");
				w.minTracklength=EditorGUILayout.IntSlider(w.minTracklength,2,10);
				EditorGUILayout.EndHorizontal();
                string[] minStrings={"Zero","Value"};
                int[] minInts={0,1};
                
                w.list1 = EditorGUILayout.IntPopup("Appears after Distance", w.list1, minStrings,minInts);

                if (w.list1 == 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
                    w.minAppearLimit = EditorGUILayout.FloatField( w.minAppearLimit);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    w.minAppearLimit = 0f;
                }
                string[] maxStrings = { "Infinity", "Value" };
                int[] maxInts = { 0, 1 };

                w.list2 = EditorGUILayout.IntPopup("Doesn't Appear after Distance", w.list2, maxStrings, maxInts);

                if (w.list2 == 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
                    w.maxAppearLimit = EditorGUILayout.FloatField(w.maxAppearLimit);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    w.maxAppearLimit = -1f;
                }
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Remove World")){
					worldmanager.Worlds.RemoveAt(i);
					
					worldmanager.showWorld.RemoveAt(i);
					break;
					
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new Track")){
					w.tracks.Add(new Tracks());
					w.showTrack.Add(false);
				}

				int j=0;
				EditorGUILayout.BeginVertical();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				foreach(Tracks t in w.tracks){


						
						w.showTrack[j]=EditorGUILayout.Foldout(w.showTrack[j],"Track "+j);
					if(w.showTrack[j]){
						EditorGUILayout.BeginHorizontal();
						GUILayout.Label("Track Probability ");
						t.probability=EditorGUILayout.Slider(t.probability,0,1);
						EditorGUILayout.EndHorizontal();

						t.track= (GameObject)EditorGUILayout.ObjectField("Track", t.track, typeof(GameObject), true);
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.Space();
						if(GUILayout.Button("Remove Track")){
							worldmanager.Worlds[i].tracks.RemoveAt(j);
							
							worldmanager.Worlds[i].showTrack.RemoveAt(j);
							break;
							
						}
						EditorGUILayout.EndHorizontal();
					}
					++j;



				}
				EditorGUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();
			}
			++i;
		}
        
				EditorGUILayout.EndVertical();
		if (GUILayout.Button("Add new World")) {
			worldmanager.Worlds.Add(new World());
			worldmanager.showWorld.Add(false);
		}
		if(GUI.changed){
			EditorUtility.SetDirty(target);
			EditorUtility.SetDirty(worldmanager);
            #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
		}
		
		worldmanagerobject.ApplyModifiedProperties();

	}
	
}