using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class LevelPopulator : MonoBehaviour
{
    public PointSets pointset;
    public int xyGridpointsCount;
    public List<xyGrid> Grid = new List<xyGrid>();
    public TrackObstacle LevelDesign;
    public TrackObstacle Obstacles;
    LevelMaker lvlmaker;

    void Start()
    {
       // Grid.Clear();
       // Grid = pointset.Grid;
        lvlmaker = GameObject.FindGameObjectWithTag("LevelMaker").GetComponent<LevelMaker>();
       
    }
   public  void RecyleObstacles()
    {
       
        LevelDesign.Recycle();
        if(Obstacles)
        Obstacles.Recycle();
    }
    public void PopulateObstacles()
    {
        try
        {
            //Grid.Clear();
            if (!lvlmaker)
                lvlmaker = GameObject.FindGameObjectWithTag("LevelMaker").GetComponent<LevelMaker>();

            PointData returnPoints = lvlmaker.PopulateLevel(pointset.Grid);
            LevelDesign.UpdatePoints(returnPoints.levelPoints);
            LevelDesign.seedobstacle();
            if (Obstacles)
            {
                Obstacles.UpdatePoints(returnPoints.obstaclePoints);
                Obstacles.seedobstacle();
            }
        }catch(System.Exception e)
        {
            Debug.LogError("Check Level Maker for empty PatternSet or Levels, check for unassigned variable in Level Populator! " + e.Data);
        }
    }




}
[System.Serializable]
public class xyGrid
{
    public List<Transform> points = new List<Transform>();
}
