using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor(typeof(PointSets))]
public class PointSetsEditor : Editor
{
    private SerializedObject pcobject;
    private PointSets pc;
    void OnEnable()
    {
        pcobject = new SerializedObject(target);
        pc = (PointSets)target;


    }
    public override void OnInspectorGUI()
    {
        pcobject.Update();
        EditorGUILayout.BeginVertical();
        GUILayout.Label(pc.points.Count.ToString() + " Grids declared!");
        GUILayout.Label("Custom points declaration");
        int i = 1;
        foreach (Points p in pc.points)
        {
            
            p.firstPoint=(GameObject)EditorGUILayout.ObjectField("Point "+i.ToString(), p.firstPoint, typeof(GameObject), true);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            pc.numpoints=EditorGUILayout.IntField("Number of Points",pc.numpoints);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            p.sepDist = EditorGUILayout.FloatField("Seperation Distance", p.sepDist);

            EditorGUILayout.EndHorizontal();
            i++;
        }
        if (GUILayout.Button("Add Point")) {
            pc.points.Add(new Points());
        }
        if (GUILayout.Button("Remove Point"))
        {
            pc.points.RemoveAt(pc.points.Count - 1);
        }

        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            pc.UpdatePoints();
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(pc);
            pcobject.ApplyModifiedProperties();
            #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }
    }
    void OnSceneGUI() {
       foreach(Points p in pc.points)
        {
            if (pc.numpoints > 0 && p.firstPoint)
            {
                Handles.color = Color.red;
                Handles.ArrowCap(0,
                        p.firstPoint.transform.position ,
                        p.firstPoint.transform.rotation * Quaternion.Euler(0, 90, 0),
                        2);
                Handles.color = Color.green;
                Handles.ArrowCap(0,
                       p.firstPoint.transform.position ,
                       p.firstPoint.transform.rotation * Quaternion.Euler(-90, 0, 0),
                       2);
                Handles.color = Color.blue;
                Handles.ArrowCap(0,
                        p.firstPoint.transform.position,
                        p.firstPoint.transform.rotation * Quaternion.Euler(0, 0,0 ),
                        2);
                for (int i = 0; i <pc.numpoints; i++)
                {
                    Handles.color = Color.red;
                    Handles.ArrowCap(0,
                            p.firstPoint.transform.position+p.firstPoint.transform.forward*(i+1)*p.sepDist,
                            p.firstPoint.transform.rotation*Quaternion.Euler(0,90,0),
                            1);
                    Handles.color = Color.green;
                    Handles.ArrowCap(0,
                           p.firstPoint.transform.position + p.firstPoint.transform.forward * (i + 1) * p.sepDist,
                           p.firstPoint.transform.rotation * Quaternion.Euler(-90, 0, 0),
                           1);
                    Handles.color = Color.blue;
                    Handles.ArrowCap(0,
                            p.firstPoint.transform.position + p.firstPoint.transform.forward* (i + 1) * p.sepDist,
                            p.firstPoint.transform.rotation * Quaternion.Euler(0, 0, 0),
                            1);
                }
            }
        }
    

    }



    }