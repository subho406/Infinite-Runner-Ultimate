using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
[RequireComponent(typeof(LevelMaker))]
public class LevelSequencer : MonoBehaviour
{
    public int BaseLevel = 0;
    int runtimeLevel = 0;
    public LevelMaker LevelMakerScript;
    float changeDistance;
    public List<BaseLevel> BaseLevels = new List<BaseLevel>();
    bool finished = false;

    void Start()
    {
        finished = false;
        runtimeLevel = 0;
        changeDistance = BaseLevels[BaseLevel].LevelSequence[runtimeLevel].nextChangeDistance;
    }

    void OnEnable()
    {
        LevelMakerScript = GetComponent<LevelMaker>();
    }
    void Update()
    {
        if (finished == false &&PlayerScoreUGUI.playerDistance() > changeDistance)
        {
            if (runtimeLevel == BaseLevels[BaseLevel].LevelSequence.Count - 1)
            {
                finished = true;
            }
            else
            {
                
             
                runtimeLevel++;
                LevelMakerScript.SetLevel(BaseLevels[BaseLevel].LevelSequence[runtimeLevel].LevelId);
                changeDistance = BaseLevels[BaseLevel].LevelSequence[runtimeLevel].nextChangeDistance;
               
            }

        }
    }
}
[System.Serializable]
public class BaseLevel
{
    public List<level> LevelSequence = new List<level>();
    public bool foldout = false;
}
    [System.Serializable]
    public class level
    {
    public string Name;
        public float nextChangeDistance = 50f;

    public int LevelId;
    public bool foldout = false;
    }

 
