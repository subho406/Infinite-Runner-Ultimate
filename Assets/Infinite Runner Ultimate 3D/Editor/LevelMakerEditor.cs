using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
[CustomEditor(typeof(LevelMaker))]
public class LevelMakerEditor : Editor
{

    private SerializedObject lvlmakerobject;
    private LevelMaker lvlmaker;
    GUIStyle headingStyle = new GUIStyle();
    GUIStyle headingStyle2 = new GUIStyle();
    GUIStyle headingStyle3 = new GUIStyle();
    void OnEnable()
    {
        lvlmakerobject = new SerializedObject(target);
        lvlmaker = (LevelMaker)target;
        headingStyle.fontStyle = FontStyle.Bold;
        headingStyle.normal.textColor = new Color(0.129f, 0.588f, 0.952f);
        headingStyle2.fontStyle = FontStyle.Bold;
        headingStyle.fontSize = 12;
        headingStyle2.normal.textColor = new Color(0.956f, 0.2627f, 0.2117f, 1);
        headingStyle2.fontSize = 16;
        headingStyle3.normal.textColor = Color.magenta;
        headingStyle3.fontSize = 12;

    }
    public void Load()
    {
        try
        {
            var path = EditorUtility.OpenFilePanel("Open Level Data", "", "bfl");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path.ToString(), FileMode.Open);
            tempSaveObject data = (tempSaveObject)bf.Deserialize(file);
            file.Close();
            lvlmaker.level.Clear();
            lvlmaker.level = data.level;
        }catch (System.Exception e)
        {
            Debug.LogError("Invalid file selected!");
        }
        
    }
    public void Save()
    {
        try
        {
            var path = EditorUtility.SaveFilePanel("Save Level Data", "", "savefile.bfl", "bfl");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path.ToString());
            tempSaveObject saveObject = new tempSaveObject();
            saveObject.level = lvlmaker.level;
            bf.Serialize(file, saveObject);
            file.Close();
        }catch(System.Exception e)
        {
            Debug.LogError("Please choose a proper file location!");
        }
    }
    public override void OnInspectorGUI()
    {
        lvlmakerobject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("Level Maker",headingStyle2);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button ("Save As")) {
            Save();
		}
        if (GUILayout.Button("Open"))
        {
            Load();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("Current Level is " + lvlmaker.currentLevel.ToString(),headingStyle);
        EditorGUILayout.Space();
        lvlmaker.xyPoints = EditorGUILayout.IntField("Point Width", lvlmaker.xyPoints);
        int i = 0;
        foreach (Levels l in lvlmaker.level)
        {
            l.foldout = EditorGUILayout.Foldout(l.foldout, i+":"+l.Name);
            if (l.foldout)
            {
                int j = 0;
                l.Name=EditorGUILayout.TextField("Level Name",l.Name);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                int r = 0;
                foreach (PatternSet ps in l.Patternset)
                {
                    for (int k = 0; k < ps.Patterns.Count; k++)
                    {
                        
                            while (ps.Patterns[k].Data.Count < lvlmaker.xyPoints)
                            {
                                ps.Patterns[k].Data.Add(new bool());

                            }
                            while (ps.Patterns[k].Data.Count > lvlmaker.xyPoints)
                        {
                            ps.Patterns[k].Data.RemoveAt(ps.Patterns[k].Data.Count - 1);
                        }
                        foreach (boolList bl in ps.Patterns[k].ObstacleData)
                        {
                            while (bl.Data.Count < lvlmaker.xyPoints)
                                bl.Data.Add(new bool());
                            while (bl.Data.Count > lvlmaker.xyPoints)
                                bl.Data.RemoveAt(bl.Data.Count - 1);
                        }

                        }
                        ps.foldout = EditorGUILayout.Foldout(ps.foldout, "Pattern Set " + j.ToString());
                    if (ps.foldout)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                    
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
      
                        EditorGUILayout.EndHorizontal();
                       
                        ps.probability = EditorGUILayout.Slider("Probability", ps.probability, 0f, 1f);
                     
                        for (int k = 0; k < ps.Patterns.Count; k++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.Space();
                            GUILayout.Label("Pattern "+k.ToString(), headingStyle);
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Label("Level Data", headingStyle3);
                            EditorGUILayout.BeginHorizontal();
                            
                            for (int x = 0; x < lvlmaker.xyPoints; x++)
                            {
                                

                                ps.Patterns[k].Data[x] = EditorGUILayout.Toggle(ps.Patterns[k].Data[x]);

                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            GUILayout.Label("Obstacle Data",headingStyle3);
                            foreach (boolList bl in ps.Patterns[k].ObstacleData)
                            {
                                EditorGUILayout.BeginHorizontal();
                                for (int x = 0; x <lvlmaker.xyPoints; x++)
                                {
                                    if (bl.Data.Count <= x)
                                    {
                                        bl.Data.Add(new bool());
                                    }
                                    bl.Data[x] = EditorGUILayout.Toggle(bl.Data[x]);
                               }

                                    EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.BeginHorizontal();
                            if (ps.Patterns[k].ObstacleData.Count < ps.Patterns[k].maxZSep)
                            {
                                if (GUILayout.Button("Add Obstacle Data"))
                                {
                                    ps.Patterns[k].ObstacleData.Add(new boolList());
                                }
                            }
                            if(GUILayout.Button("Random Fill"))
                            {
                                int counter = 0;
                                foreach (boolList bl in ps.Patterns[k].ObstacleData)
                                {
                                    for (int x = 0; x < lvlmaker.xyPoints; x++)
                                    {
                                        float val = Random.value;
                                        if (val > 0.5f)
                                            bl.Data[x] = true;
                                        else
                                            bl.Data[x] = false;
                                    }
                                    }
                                counter++;
                                }
                            if (ps.Patterns[k].ObstacleData.Count > 0)
                            {
                                if (GUILayout.Button("Remove Obstacle Data"))
                                {
                                    ps.Patterns[k].ObstacleData.RemoveAt(ps.Patterns[k].ObstacleData.Count - 1);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            ps.Patterns[k].minZSep = EditorGUILayout.IntField("Min Z Sep", ps.Patterns[k].minZSep);
                            ps.Patterns[k].maxZSep = EditorGUILayout.IntField("Max Z Sep", ps.Patterns[k].maxZSep);
                            EditorGUILayout.EndHorizontal();
                          
                            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                            EditorGUILayout.Space();
                        }
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Pattern Data"))
                        {
                            pattern pat = new pattern();
                            pat.Count = lvlmaker.xyPoints;
                            ps.Patterns.Add(pat);

                        }

                        if (ps.Patterns.Count - 1 > 0 && GUILayout.Button("Remove Pattern Data"))
                            ps.Patterns.RemoveAt(ps.Patterns.Count - 1);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                        //GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Space();
                   
                    if (!ps.foldout&&GUILayout.Button("Remove PatternSet "+r.ToString()))
                    {
                        l.Patternset.RemoveAt(r);
                        break;
                    }
                    EditorGUILayout.Space();
                    j++;
					r++;

                }
                EditorGUILayout.Space();
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                EditorGUILayout.Space(); EditorGUILayout.Space();
                if (GUILayout.Button("Add new PatternSet"))
                    l.Patternset.Add(new PatternSet());
           
                EditorGUILayout.BeginHorizontal();
                l.obsData=EditorGUILayout.IntField("Obstacle Count", l.obsData);
                if(GUILayout.Button("Set obstacle data"))
                {
                    foreach(PatternSet p in l.Patternset)
                    {
                        foreach(pattern pat in p.Patterns)
                        {
                            pat.minZSep = l.obsData;
                            pat.maxZSep = l.obsData + 1;
                            if (pat.ObstacleData.Count > pat.maxZSep)
                            {
                                while (pat.ObstacleData.Count > pat.maxZSep)
                                {
                                    pat.ObstacleData.RemoveAt(pat.ObstacleData.Count - 1);
                                }
                            }
                            else
                            {
                                while(pat.ObstacleData.Count < pat.maxZSep)
                                {
                                    pat.ObstacleData.Add(new boolList());
                                    for (int x = 0; x < lvlmaker.xyPoints; x++)
                                        pat.ObstacleData[pat.ObstacleData.Count - 1].Data.Add(false);
                                }
                            }

                        }
                    }
                    Debug.Log(l.Patternset[0].Patterns[0].ObstacleData.Count);

                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(lvlmaker);
                      #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                GUILayout.Label("Special filling tools",headingStyle3);
                l.factor = EditorGUILayout.IntField("Up Factor", l.factor);
                l.downfactor = EditorGUILayout.IntField("Down Factor", l.downfactor);
                l.probability = EditorGUILayout.FloatField("Up Probability", l.probability);
                l.innerProbab= EditorGUILayout.FloatField("Middle Probability", l.innerProbab);
                l.downProbab = EditorGUILayout.FloatField("Down Probability", l.downProbab);
                if (GUILayout.Button("Special Fill"))
                {
                    foreach (PatternSet p in l.Patternset)
                    {
                        //Reseting all values
                        foreach (pattern pat in p.Patterns)
                            foreach (boolList b in pat.ObstacleData)
                                for (int y = 0; y < b.Data.Count; y++)
                                    b.Data[y] = false;
                        //Reseting all values end
                        for(int x = 0; x < p.Patterns.Count; x++)
                        {
                           for(int y = 0; y < p.Patterns[x].Data.Count; y++)
                            {
                                if (!p.Patterns[x].Data[y])
                                {
                                    for(int z = 0; z < l.factor; z++)
                                    {
                                       
                                        if (Random.value <l.probability)
                                            p.Patterns[x].ObstacleData[z].Data[y] = true;
                                        else
                                            p.Patterns[x].ObstacleData[z].Data[y] = false;

                                    }
                                }else
                                {
                                    for (int z = 0; z < l.factor; z++)
                                    {
                                        p.Patterns[x].ObstacleData[z].Data[y] = false;
                                    }
                                    }
                                    
                            }
                           for(int y = l.factor; y < p.Patterns[x].ObstacleData.Count - l.downfactor ;y++)
                            {
                                for(int z = 0; z < p.Patterns[x].ObstacleData[y].Data.Count; z++)
                                {
                                    if (Random.value<l.innerProbab)
                                        p.Patterns[x].ObstacleData[y].Data[z] = true;
                                    else
                                        p.Patterns[x].ObstacleData[y].Data[z] = false;
                                }
                            }
                            if (x + 1 < p.Patterns.Count)
                            {
                                for (int y = 0; y <lvlmaker.xyPoints; y++)
                                {
                                    if (!p.Patterns[x+1].Data[y])
                                    {
                                        for (int z = p.Patterns[x].ObstacleData.Count-1; z > p.Patterns[x].ObstacleData.Count-l.downfactor-1; z--)
                                        {

                                            if (Random.value <l.downProbab)
                                                p.Patterns[x].ObstacleData[z].Data[y] = true;
                                            else
                                                p.Patterns[x].ObstacleData[z].Data[y] = false;

                                        }
                                    }
                                    else
                                    {
                                        for (int z = p.Patterns[x].ObstacleData.Count - 1; z > p.Patterns[x].ObstacleData.Count - l.downfactor - 1; z--)
                                        {
                                           
                                            p.Patterns[x].ObstacleData[z].Data[y] = false;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(lvlmaker);
                      #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif

                }
              
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Level"))
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new MemoryStream();
                    formatter.Serialize(stream, l);
                    stream.Seek(0, SeekOrigin.Begin);
                    Levels copyLevel = (Levels)formatter.Deserialize(stream);
                    lvlmaker.level.Insert(i,copyLevel);
                    break;
                }
                if (GUILayout.Button("Remove Level"))
                {
                    lvlmaker.level.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

            }
            i++;

        }
        EditorGUILayout.Space(); EditorGUILayout.Space();
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add new Level"))
            lvlmaker.level.Add(new Levels());
   
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(lvlmaker);
              #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }

        lvlmakerobject.ApplyModifiedProperties();
    }

}
[System.Serializable]
class tempSaveObject{
	public List<Levels> level=new List<Levels>();
}
