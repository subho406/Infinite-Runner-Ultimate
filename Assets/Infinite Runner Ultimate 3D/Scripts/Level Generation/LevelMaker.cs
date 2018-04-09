/*

Level Maker Beta
                 - Programmed by Subhojeet 
A BornFree Labs Production

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class LevelMaker : MonoBehaviour
{
   
    public PropPoolManager poolManager;
    public int currentLevel = 0;
    public List<Levels> level = new List<Levels>();
    public int xyPoints = 3;
    public int patternProgress = 0;
   public  int currentPatternSet = -1;
  public  int obstacleProgress = -1;
   public int obstacleMax = 0;
    public int skipPointMax = 1;  //Beta Stage
    public int currentPattern = 0;
    List<float> probabs=new List<float>();
    public static List<Vector3> previouspoint;
    void Start()
    {
        previouspoint = new List<Vector3>();
        skipPointMax = 1;
        patternProgress = 0;
       foreach(PatternSet ps in level[currentLevel].Patternset)
        {
            probabs.Add(ps.probability);
        }
        currentPatternSet = Probability(probabs, level[currentLevel].Patternset.Count);
        obstacleProgress = -1;

    }
    void Update()
    {
        if(LevelMaker.previouspoint.Count>30)
        {
            while (previouspoint.Count != 30)
            {
                previouspoint.RemoveAt(0);
            };
        }
    }
    public void IncrementLevel(){
		
			if (currentLevel < level.Count - 1) {
				currentLevel++;
				probabs.Clear ();
				foreach(PatternSet ps in level[currentLevel].Patternset)
				{
					probabs.Add(ps.probability);
				}
			}
		
	}
    public void SetLevel(int id)
    {
        currentLevel = id;
        probabs.Clear();
        foreach (PatternSet ps in level[currentLevel].Patternset)
        {
            probabs.Add(ps.probability);
        }
    }


    public PointData PopulateLevel(List<xyGrid> grid)
    {
        int count = grid.Count;
        PointData Data=new PointData();
 
        for (int i = 0; i < count; i++)
        {

            Start:
 
                if (currentPattern >= level[currentLevel].Patternset[currentPatternSet].Patterns.Count)
                {
                    obstacleProgress = -1;
                    currentPattern = 0;
                    currentPatternSet = Probability(probabs, probabs.Count);

                }
                if (obstacleProgress == -1)
                {
                    for (int j = 0; j < xyPoints; j++)
                    {
                        if (level[currentLevel].Patternset[currentPatternSet].Patterns[currentPattern].Data[j])
                            Data.levelPoints.Add(grid[i].points[j]);

                    }

                    obstacleProgress = 0;
                    obstacleMax = Random.Range(level[currentLevel].Patternset[currentPatternSet].Patterns[currentPattern].minZSep, level[currentLevel].Patternset[currentPatternSet].Patterns[currentPattern].maxZSep);
             
                   

                }
                else
                {
                    if (obstacleProgress > obstacleMax)
                    {
                        obstacleProgress = -1;

                        currentPattern++;

                    goto Start;
                       
                    }
                    else
                    {
                        if (obstacleProgress < level[currentLevel].Patternset[currentPatternSet].Patterns[currentPattern].ObstacleData.Count)
                        {
                            for (int j = 0; j < xyPoints; j++)
                            {
                                if (level[currentLevel].Patternset[currentPatternSet].Patterns[currentPattern].ObstacleData[obstacleProgress].Data[j])
                                {
                                    Data.obstaclePoints.Add(grid[i].points[j]);
                                }
                            }
                            }
                     
                            obstacleProgress++;
                        
                    }
                }



            }
        
        return Data;
    }

    //Maths functions sections.
    int Probability(List<float> probabs, int size)
    {
        float sum = 0f;
        for (int i = 0; i < size; ++i)
        {
            sum += probabs[i];
        }
        float value = sum * Random.value;

        float sum2 = probabs[0];
        for (int i = 0; i < size; ++i)
        {
            if (sum2 > value)
            {
                return 0;
            }
            else if (size - i == 1)
            {
                return i;
            }
            else if (value >= sum2 && value < sum2 + probabs[i + 1])
            {
                return i + 1;
            }
            sum2 += probabs[i + 1];
        }
        return 0;


    }

}
[System.Serializable]
public class pattern
{
    public List<bool> Data = new List<bool>();
    public List<boolList> ObstacleData = new List<boolList>();
    public int minZSep = 1;
    public int maxZSep = 5;

    public int Count = 0;
}
[System.Serializable]
public class boolList
{
    public List<bool> Data = new List<bool>();

}
[System.Serializable]
public class PatternSet
{

    public float probability = 1f;
    public List<pattern> Patterns = new List<pattern>();
   
    public bool foldout = false;
}

[System.Serializable]
public class Levels
{
    public string Name;
    public List<PatternSet> Patternset = new List<PatternSet>();
    public bool foldout = false;
    public int obsData = 5;
    public int factor = 3;
    public int downfactor = 3;
    public float probability = 0.5f;
    public float downProbab = 0.5f;
    public float innerProbab = 0.5f;

}
[System.Serializable]
public class PointData
{
    public List<Transform> levelPoints=new List<Transform>();
    public List<Transform> obstaclePoints=new List<Transform>();
}
