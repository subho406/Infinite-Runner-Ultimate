using UnityEngine;
using System.Collections;
using UnityEditor;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor(typeof(LevelSequencer))]
public class LevelSequencerEditor : Editor
{
    private SerializedObject lvlseqobject;
    private LevelSequencer lvlseq;
    GUIStyle headingStyle3 = new GUIStyle();
    void OnEnable()
    {
        lvlseqobject = new SerializedObject(target);
        lvlseq = (LevelSequencer)target;
        headingStyle3.normal.textColor = Color.magenta;
        headingStyle3.fontSize = 12;
    }
    public override void OnInspectorGUI()
    {
        lvlseqobject.Update();
        EditorGUILayout.BeginVertical();
        lvlseq.BaseLevel = EditorGUILayout.IntField("Current Meta Level", lvlseq.BaseLevel);
        EditorGUILayout.BeginHorizontal();EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < lvlseq.BaseLevels.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            lvlseq.BaseLevels[i].foldout = EditorGUILayout.Foldout(lvlseq.BaseLevels[i].foldout, "");
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Meta Level " + i, headingStyle3);
            if (lvlseq.BaseLevels[i].foldout)
            {
                EditorGUILayout.BeginHorizontal(); EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                int s = 0;
                foreach (level l in lvlseq.BaseLevels[i].LevelSequence)
                {
                    l.foldout = EditorGUILayout.Foldout(l.foldout, l.Name+": CD-"+l.nextChangeDistance.ToString());
                    if (l.foldout)
                    {
                        if (!lvlseq.LevelMakerScript)
                            lvlseq.LevelMakerScript = lvlseq.GetComponent<LevelMaker>();
                        int[] ids = new int[lvlseq.LevelMakerScript.level.Count];
                        string[] names = new string[lvlseq.LevelMakerScript.level.Count];
                        for (int j = 0; j < lvlseq.LevelMakerScript.level.Count; j++)
                        {
                            ids[j] = j;
                            names[j] = lvlseq.LevelMakerScript.level[j].Name;
                        }
                        l.LevelId = EditorGUILayout.IntPopup("Sub Level", l.LevelId, names, ids);
                        l.Name = names[l.LevelId];
                        l.nextChangeDistance = EditorGUILayout.FloatField("Next change distance", l.nextChangeDistance);
                      
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add"))
                        {
                            lvlseq.BaseLevels[i].LevelSequence.Insert(s + 1, new level());
                            break;
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            lvlseq.BaseLevels[i].LevelSequence.RemoveAt(s); break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }


                    s++;
                }
                if (GUILayout.Button("Add Sub Level"))
                    lvlseq.BaseLevels[i].LevelSequence.Add(new level());
                if (GUILayout.Button("Remove Meta Level"))
                {
                    lvlseq.BaseLevels.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if(GUILayout.Button("Add Meta Level"))
        {
            lvlseq.BaseLevels.Add(new BaseLevel());
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(lvlseq);
                 #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }

        lvlseqobject.ApplyModifiedProperties();
    }

    }
