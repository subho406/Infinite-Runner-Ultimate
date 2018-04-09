using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class PointSets : MonoBehaviour
{
    public List<Points> points = new List<Points>();
    public List<xyGrid> Grid = new List<xyGrid>();
    public int numpoints;
    public void UpdatePoints()

    {
        foreach (xyGrid xy in Grid)
        {
            foreach(Transform p in xy.points)
            {
                if(p)
                DestroyImmediate(p.gameObject);
            }
        }
        Grid.Clear();


        for (int i = 1; i <= numpoints; i++)
        {
            Grid.Add(new xyGrid());
            for (int j = 0; j < points.Count; j++)
            {
                Points p = points[j];
                GameObject g = new GameObject();
                g.transform.position = p.firstPoint.transform.position + p.firstPoint.transform.forward * p.sepDist * i;
                g.transform.rotation = p.firstPoint.transform.rotation;
                g.transform.parent = p.firstPoint.transform;
                Grid[i - 1].points.Add(g.transform);
            }
        }


    }
    public List<Transform> returnPoints()
    {
        List<Transform> points = new List<Transform>();
        foreach(xyGrid gr in Grid)
        {
            foreach (Transform t in gr.points)
                points.Add(t);
        }
        return points;
    }

}

[System.Serializable]
public class Points
{
    public GameObject firstPoint;
    public List<GameObject> pts = new List<GameObject>();

    public float sepDist = 2f;


}
