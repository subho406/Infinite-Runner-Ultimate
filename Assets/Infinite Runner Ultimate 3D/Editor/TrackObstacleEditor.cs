using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
#if !(UNITY_5_0||UNITY_5_1||UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor(typeof(TrackObstacle))]
public class TrackObstacleEditor : Editor
{
    Texture2D image;

    void Awake()
    {
        image = Resources.Load("arrow.png", typeof(Texture2D)) as Texture2D;
    }

    [MenuItem("GameObject/Create Other/Infinite Runner Tool/Track Obstacle mesh")]
    static void CreatePrefab()
    {
        TrackObstacle n = new TrackObstacle();
        n.createm();

    }


    private SerializedObject trackobstacleobject;
    private TrackObstacle trackobstacle;

    void OnEnable()
    {
        trackobstacleobject = new SerializedObject(target);
        trackobstacle = (TrackObstacle)target;


    }

    public override void OnInspectorGUI()
    {
        trackobstacleobject.Update();
        EditorGUILayout.BeginVertical();
        trackobstacle.staticSeperation = EditorGUILayout.Toggle("Global Seperation between obstacles", trackobstacle.staticSeperation);
        trackobstacle.useother = EditorGUILayout.Toggle("Use Other Pool Manager", trackobstacle.useother);
        EditorGUILayout.BeginHorizontal();

        trackobstacle.useCustomPoints = EditorGUILayout.Toggle("Use Custom Points", trackobstacle.useCustomPoints);
        EditorGUILayout.EndHorizontal();
        if (trackobstacle.useCustomPoints == false)
            trackobstacle.gam = (GameObject)EditorGUILayout.ObjectField("Mesh GameObject", trackobstacle.gam, typeof(GameObject), true);
        else
        {
            trackobstacle.pointSet = (PointSets)EditorGUILayout.ObjectField("Set of points", trackobstacle.pointSet, typeof(PointSets), true);
            if (GUILayout.Button("Update Points"))
            {
                trackobstacle.points.Clear();
                trackobstacle.points=trackobstacle.pointSet.returnPoints();
            }
            trackobstacle.PointsFoldout = EditorGUILayout.Foldout(trackobstacle.PointsFoldout, "Points");
            if (trackobstacle.PointsFoldout)
            {
                for (int x = 0; x < trackobstacle.points.Count; x++)
                {
                    trackobstacle.points[x] = (Transform)EditorGUILayout.ObjectField("Transform " + (x + 1).ToString(), trackobstacle.points[x], typeof(Transform), true);

                }
            }
        }
        EditorGUILayout.Space(); EditorGUILayout.Space();
        if (trackobstacle.useother == false)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Pooled Amount ");

            trackobstacle.PooledAmount = EditorGUILayout.IntField(trackobstacle.PooledAmount);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (!GameObject.FindWithTag("PoolManager"))
            {
                GUILayout.Label("Please Tag the Pool Manager as PoolManager");
            }
            else
            {
                trackobstacle.poolManager = (PropPoolManager)GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PropPoolManager>();

            }
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Obstacle Appearence probability");
        trackobstacle.mainprobab = EditorGUILayout.Slider(trackobstacle.mainprobab, 0, 1);
        EditorGUILayout.EndHorizontal();
        if (trackobstacle.useCustomPoints == false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Frames Mod");
            trackobstacle.FramesMod = EditorGUILayout.IntField(trackobstacle.FramesMod);
            EditorGUILayout.EndHorizontal();
        }

        int i = 0;
        foreach (Obstacles ob in trackobstacle.obstacles)
        {
            trackobstacle.showobject[i] = EditorGUILayout.Foldout(trackobstacle.showobject[i], "Obstacle " + i);
            if (trackobstacle.showobject[i])
            {


                if (trackobstacle.useother == false)
                {
                    ob.obstacle = (GameObject)EditorGUILayout.ObjectField("Obstacle", ob.obstacle, typeof(GameObject), true);
                }
                else
                {
                    if (trackobstacle.poolManager)
                    {
                        string[] s = new string[trackobstacle.poolManager.Names.Count];
                        for (int j = 0; j < trackobstacle.poolManager.Names.Count; ++j)
                        {
                            s[j] = trackobstacle.poolManager.Names[j];
                        }
                        int[] num = new int[trackobstacle.poolManager.Names.Count];
                        for (int j = 0; j < trackobstacle.poolManager.Names.Count; ++j)
                        {
                            num[j] = j;
                        }
                        ob.otherID = EditorGUILayout.IntPopup("Obstacle Object", ob.otherID, s, num);


                    }
                    else
                    {
                        GUILayout.Label("Please assign a Pool Manager");

                    }
                }
                EditorGUILayout.BeginHorizontal();
                ob.ZingVariableObject = EditorGUILayout.Toggle("Zing variable Object", ob.ZingVariableObject);
                if (ob.ZingVariableObject)
                    ob.Name = EditorGUILayout.TextField("Unique Name", ob.Name);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Probability");
                ob.probability = EditorGUILayout.Slider(ob.probability, 0, 1);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Seperation Distance");
                ob.sepdist = EditorGUILayout.FloatField(ob.sepdist);
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Specific Seperation");
                foreach (Specific_Seperation s in ob.SpecificSeperation)
                {
                    EditorGUILayout.BeginHorizontal();
                    s.index = EditorGUILayout.IntField("Obstacle ", s.index);
                    if (s.index < 0)
                        s.index = 0;
                    else if (s.index > trackobstacle.obstacles.Count)
                        s.index = trackobstacle.obstacles.Count - 1;
                    s.sepDist = EditorGUILayout.FloatField("Sep Dist", s.sepDist);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add new specific seperation"))
                {
                    ob.SpecificSeperation.Add(new Specific_Seperation());
                }
                if (GUILayout.Button("Remove"))
                {
                    ob.SpecificSeperation.RemoveAt(ob.SpecificSeperation.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                ob.trans = (TrackObstacle.transenum)EditorGUILayout.EnumPopup("Object Fixed Transform ", ob.trans);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Procedures");
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                int k = 0;
                foreach (Procedures p in ob.procedures)
                {
                    //Procedure Section
                    string[] str = new string[9] { "Align With Mesh Normals", "Rotate With Vector", "Move With Vector", "Rotate With Random Vector", "Move With Random Vector", "Look At Transform", "Translate With Random Vector", "Translate With Vector", "Rotate With Other Transform" };
                    ob.showProcedures[k] = EditorGUILayout.Foldout(ob.showProcedures[k], str[p.id]);
                    if (ob.showProcedures[k])
                    {
                        int[] pop = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };


                        p.id = EditorGUILayout.IntPopup(p.id, str, pop);
                        switch (p.id)
                        {
                            case 0:
                                GUILayout.Label("Align with mesh Normals");
                                break;
                            case 1:
                                p.rot = EditorGUILayout.Vector3Field("Rotation Vector", p.rot);
                                break;
                            case 2:
                                p.add = EditorGUILayout.Vector3Field("Move Vector ", p.add);
                                break;
                            case 3:
                                GUILayout.Label("Random Rotation Range");
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                p.RandomRotmin = EditorGUILayout.Vector3Field("Min Value", p.RandomRotmin);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                p.RandomRotmax = EditorGUILayout.Vector3Field("Max Value", p.RandomRotmax);
                                EditorGUILayout.EndHorizontal();
                                break;
                            case 4:
                                GUILayout.Label("Random Vector Range");
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                p.RandomPosmin = EditorGUILayout.Vector3Field("Min Value", p.RandomPosmin);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                p.RandomPosmax = EditorGUILayout.Vector3Field("Max Value", p.RandomPosmax);
                                EditorGUILayout.EndHorizontal();
                                break;
                            case 5:
                                p.rotaxis = (Transform)EditorGUILayout.ObjectField("Object Look Rotation ", p.rotaxis, typeof(Transform), true);
                                break;
                            case 6:
                                GUILayout.Label("Random Transform Range");
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                p.Randomtransmin = EditorGUILayout.Vector3Field("Min Value", p.Randomtransmin);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                p.Randomtransmax = EditorGUILayout.Vector3Field("Max Value", p.Randomtransmax);
                                EditorGUILayout.EndHorizontal();
                                break;
                            case 7:
                                p.TransformVector = EditorGUILayout.Vector3Field("Translate Vector ", p.TransformVector);
                                break;
                            case 8:
                                p.rotatevector = (Transform)EditorGUILayout.ObjectField("Rotate Transform", p.rotatevector, typeof(Transform), true);
                                break;


                        };
                        image = Resources.Load("arrow.png", typeof(Texture2D)) as Texture2D;
                        GUILayout.Label(image);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Remove Procedure"))
                        {
                            ob.procedures.RemoveAt(k);
                            ob.showProcedures.RemoveAt(k);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();

                    }
                    ++k;
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Add new Procedure"))
                {
                    trackobstacle.obstacles[i].procedures.Add(new Procedures());
                    trackobstacle.obstacles[i].showProcedures.Add(false);
                }



                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Remove Obstacle"))
                {
                    trackobstacle.obstacles.RemoveAt(i);

                    trackobstacle.showobject.RemoveAt(i);
                    break;

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            ++i;
        }
        if (GUILayout.Button("Add new Obstacle"))
        {
            trackobstacle.obstacles.Add(new Obstacles());
            trackobstacle.showobject.Add(false);
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(trackobstacle);
            #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }

        trackobstacleobject.ApplyModifiedProperties();

    }

}
