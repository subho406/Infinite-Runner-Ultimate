using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
#if !(UNITY_5_0||UNITY_5_1||UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor(typeof(TrackObject))]
public class TrackObjectEditor : Editor
{
    private SerializedObject pcobject;
    private TrackObject pc;
    void OnEnable()
    {
        pcobject = new SerializedObject(target);
        pc = (TrackObject)target;


    }
  
    public override void OnInspectorGUI()
    {
        pcobject.Update();
    
        DrawDefaultInspector();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(); EditorGUILayout.Space();

        GUILayout.Label("                ADVANCED SETUP");
        EditorGUILayout.Space();
        pc.LimitSpeed = EditorGUILayout.Toggle("Limit Speed", pc.LimitSpeed);
        if (pc.LimitSpeed)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            pc.LimitedSpeed = EditorGUILayout.FloatField("Limited Speed", pc.LimitedSpeed);
            EditorGUILayout.EndHorizontal();
        }
        pc.UseWaypoint = EditorGUILayout.Toggle("Use Waypoint", pc.UseWaypoint);
        if (pc.UseWaypoint)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            pc.WaypointProgress = (WaypointProgressTracker)EditorGUILayout.ObjectField("Waypoint Progress Tracker Script", pc.WaypointProgress, typeof(WaypointProgressTracker), true);
            EditorGUILayout.EndHorizontal();
        }
        pc.UseMultipleRefPoints = EditorGUILayout.Toggle("Use multiple Next Track Points", pc.UseMultipleRefPoints);
        if (pc.UseMultipleRefPoints)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            string[] sizes ={"2","3"};
            int[] actualsize={2,3};
            pc.numPoints = EditorGUILayout.IntPopup("Number of Points", pc.numPoints, sizes, actualsize);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
           
            for (int i = 0; i < pc.numPoints; i++)
            {
                pc.nextTrackPoints[i] = (GameObject)EditorGUILayout.ObjectField("Point "+i, pc.nextTrackPoints[i], typeof(GameObject), true);
                pc.ExitTriggers[i] = (GameObject)EditorGUILayout.ObjectField("Exit Trigger " + i, pc.ExitTriggers[i], typeof(GameObject), true);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();  
           
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            pc.NextTrackRefPoint = (GameObject)EditorGUILayout.ObjectField("Next Track Ref Point", pc.NextTrackRefPoint, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(pc);
            pcobject.ApplyModifiedProperties();
            #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            #endif
        }
       
    }

}
