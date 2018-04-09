/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script creates and recycles objects at runtime based on given procedures


*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TrackObstacle : MonoBehaviour {
	public bool useother=false;
    public bool staticSeperation = false;
	public PropPoolManager poolManager;
	public List<Obstacles> obstacles=new List<Obstacles>();
	public List<bool>showobject=new List<bool>();
	public float mainprobab=0.5f;
	public enum transenum {X=0, Y=1,Z=2, None=3};
	public int FramesMod=2;
	public int currentMin=0;
    public bool PointsFoldout = false;
	public int AddValue=0;
	public int currentMax=0;
	private bool Worktodo=false;
	public GameObject gam;
	public Mesh mesh;
	bool probabnotadded=false;
	public int PooledAmount=2;
	Vector3[] verts;
    public List<Transform> points=new List<Transform>();
    public PointSets pointSet;
    public int pointsCount = 0;
    public bool useCustomPoints = false;
	List<float> probabs=new List<float>();
	int[] indices;
	GameObject temp;
    public List<int> updatableIndexes = new List<int>();
    List<Vector3> previouspoint = new List<Vector3>();
    List<int> obstacleIndex = new List<int>();
	#if UNITY_EDITOR
	public void createm (){
        
		GameObject Cube=new GameObject();
		GameObject plane=GameObject.CreatePrimitive(PrimitiveType.Plane);
		DestroyImmediate(plane.GetComponent<MeshCollider>());
		Selection.activeObject = SceneView.currentDrawingSceneView;

		Camera sceneCam = SceneView.currentDrawingSceneView.camera;
		Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,10f));
		
		plane.transform.position=spawnPos;
			Cube.transform.position=spawnPos;
		Cube.transform.rotation=Quaternion.identity;
		plane.transform.rotation=Cube.transform.rotation;
		
		plane.transform.parent=Cube.transform;
		Cube.AddComponent<TrackObstacle>();
		Cube.GetComponent<TrackObstacle>().gam=plane;
		Cube.name="TrackObstacle";
		plane.name="mesh";
		PrefabUtility.InstantiatePrefab(Cube);
	}
#endif

    	void Awake() {
        
		//Always reset scale of transform object to Unity to avoid bugs
		probabnotadded=false;
        if(useCustomPoints==false)
		mesh=gam.GetComponent<MeshFilter>().mesh;

        if (staticSeperation)
            previouspoint = LevelMaker.previouspoint;
        else
		    previouspoint=new List<Vector3>();
		if(useother==false){
			
			if(mesh){
				
				
				
				for(int i=0; i<obstacles.Count;++i){
					for(int j=0; j<PooledAmount; ++j){
						obstacles[i].poolTrack.Add((GameObject)Instantiate(obstacles[i].obstacle,Vector3.zero,Quaternion.identity));
						obstacles[i].poolTrack[j].transform.parent=transform;
						
						obstacles[i].poolTrack[j].SetActive(false);
					}
				}
				
			}else{
				Debug.LogError("Mesh Has not been assigned");
			}
		}
		if(useother==true){
			poolManager=GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PropPoolManager>();
		}

		}
	void Start(){
            transform.localScale=Vector3.one;
	}

    void LateUpdate(){
	if(Worktodo==true){
           // Recycle();
			seedobstacle();
		}
	}

	public void Recycle(){
		if(useother==false){
		for(int i=0; i<obstacles.Count;++i){
			for(int j=0; j<obstacles[i].poolTrack.Count; ++j){
				obstacles[i].poolTrack[j].SetActive(false);
			}
		}
        if(!staticSeperation)
		previouspoint.Clear();
            obstacleIndex.Clear();
		}else {
			for(int i=0; i<obstacles.Count;++i){
				for(int j=0; j<obstacles[i].poolTrack.Count;++j){
					obstacles[i].poolTrack[j].SetActive(false);
					obstacles[i].poolTrack[j].transform.parent=poolManager.transform;
				}
				obstacles[i].poolTrack.Clear();

			}
            if(!staticSeperation)
			previouspoint.Clear ();
            obstacleIndex.Clear();
        }


	}
    public void UpdatePoints(List<Transform> transforms)
    {
        points.Clear();
        foreach(Transform t in transforms)
        {
            points.Add(t);
        }
   


    }
    public void seedobstacle() {

		if(probabnotadded==false)
        {
            if (useCustomPoints == false)
            {
                verts = mesh.vertices;
                indices = mesh.triangles;
            }
       
            poolManager =GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PropPoolManager>();
	
			temp=new GameObject();
			temp.transform.parent=transform;
			probabnotadded=true;
            for (int i = 0; i < obstacles.Count; ++i)          //Probabilities are variables hence need to be added evrytime
            {
                probabs.Add(obstacles[i].probability);
                if (obstacles[i].ZingVariableObject)  //If it is a variable object then add its index so that next time when seedobstacle is called Probabilities are updated
                    updatableIndexes.Add(i); 
            }

        }
   
        if (useCustomPoints == false)
        {
            if (Worktodo == false)
            {
                Worktodo = true;
                currentMin = 0;
                AddValue = mesh.triangles.Length / FramesMod;
                currentMax = mesh.triangles.Length / FramesMod;
            }
            if (currentMax > mesh.triangles.Length)
            {
                Worktodo = false;
                return;
            }
        }
        else
        {
            if (Worktodo == false)
            {
                Worktodo = true;
                currentMin = 0;
                AddValue = points.Count;
                currentMax = points.Count;
            }
            if (currentMax > points.Count)
            {
                Worktodo = false;
                return;
            }
        }
       
        
		for(int i = 0; i < currentMax;)
        {
            Vector3 pos, rot;
            if (useCustomPoints == false)
            {
                Vector3 P1 = verts[indices[i++]];

                Vector3 P2 = verts[indices[i++]];

                Vector3 P3 = verts[indices[i++]];

                 pos = gam.transform.TransformPoint((P1 + P2 + P3) / 3);
                Vector3 n1 = verts[indices[i++]];

                Vector3 n2 = verts[indices[i++]];

                Vector3 n3 = verts[indices[i++]];
              rot = gam.transform.TransformDirection((n1 + n2 + n3) / 3);
            }
            else
            {
                
                pos = points[currentMin+i].position;
                rot = points[currentMin+i].position;
                i++;
                
            }
			int Randtrack=Probability(probabs,probabs.Count);
            if (Randtrack == -1)
            {
                Worktodo = false;
                return;

            }
                bool b=true;
			transenum trans=obstacles[Randtrack].trans;
			float value=Random.value;
	



			temp.transform.position=pos;
            if (useCustomPoints == true)
                temp.transform.rotation = points[currentMin + i - 1].rotation;
            else
			temp.transform.localRotation=Quaternion.identity;
						

						switch (trans){
						case transenum.X:
							temp.transform.localPosition=new Vector3(0,temp.transform.localPosition.y,temp.transform.localPosition.z);
							break;
						case transenum.Y:
							temp.transform.localPosition=new Vector3(temp.transform.localPosition.x,0,temp.transform.localPosition.z);
							break;
						case transenum.Z:
							temp.transform.localPosition=new Vector3(temp.transform.localPosition.x,temp.transform.localPosition.y,0);
							break;
						case transenum.None:

							break;
						default:
							break;
							
						}
					for(int x=0; x<obstacles[Randtrack].procedures.Count;++x){
						doProcedure(obstacles[Randtrack].procedures[x],temp,rot);
					}
            int j = previouspoint.Count - 10;
            if (j < 0)
                j = 0;
			for(j=0; j<previouspoint.Count;++j){
                bool contains = false;
                bool canContinue = true;
                for (int x = 0; x < obstacles[Randtrack].SpecificSeperation.Count; x++)
                {
                    
                    if (obstacles[Randtrack].SpecificSeperation[x].index == obstacleIndex[j])
                    {
                        contains = true;
                        if (Vector3.Distance(temp.transform.position, previouspoint[j]) < obstacles[Randtrack].SpecificSeperation[x].sepDist)
                        {
                            b = false;
                            canContinue = false;
                            break;
                        }
                    }
                }
			if(contains==false&&Vector3.Distance(temp.transform.position,previouspoint[j])<obstacles[Randtrack].sepdist){
					b=false;
					break;
				}
                if (!canContinue)
                    break;
				   }
			if(b==true) {
				if(value<mainprobab){
					GameObject temp2=CheckPool(Randtrack);
					temp2.transform.position=temp.transform.position;
					temp2.transform.rotation=temp.transform.rotation;
					previouspoint.Add(temp2.transform.position);
                    obstacleIndex.Add(Randtrack);
				   temp2.SetActive(true);
					temp2.transform.parent=transform;
					obstacles[Randtrack].poolTrack.Add(temp2);
				}else{
					//temp.transform.parent=GameObject.FindGameObjectWithTag("PoolManager").transform;
					//temp.SetActive(false);

				}
                

			}else {

				//temp.transform.parent=GameObject.FindGameObjectWithTag("PoolManager").transform;
				//temp.SetActive(false);
			}

			
			

		}
        if (useCustomPoints == false) { 
		currentMin+=AddValue;
		if(currentMax==mesh.triangles.Length){
			currentMax+=1;
		}
		else if(currentMax+AddValue>mesh.triangles.Length){
			currentMax=mesh.triangles.Length;
		}else{
		currentMax+=AddValue;
		}

        }
        else
        {
            Worktodo = false;
            return;
        }
   

	}
	public GameObject CheckPool (int randtrack){
		if(useother==true){

			for(int i=0; i<poolManager.pObstacle[obstacles[randtrack].otherID].pools.Count;++i){
				if(poolManager.pObstacle[obstacles[randtrack].otherID].pools[i].activeInHierarchy==false){
					return poolManager.pObstacle[obstacles[randtrack].otherID].pools[i];
				}
			}
		} else {
		for(int i=0; i<obstacles[randtrack].poolTrack.Count;++i){
		if(obstacles[randtrack].poolTrack[i].activeInHierarchy==false){
				return obstacles[randtrack].poolTrack[i];
			}
			} 
		}
		if(useother==false){
		obstacles[randtrack].poolTrack.Add((GameObject)Instantiate(obstacles[randtrack].obstacle,Vector3.zero,Quaternion.identity));
		obstacles[randtrack].poolTrack[obstacles[randtrack].poolTrack.Count-1].transform.parent=transform;
		}else {
			poolManager=GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PropPoolManager>();
			GameObject temp=(GameObject)Instantiate(poolManager.pObstacle[obstacles[randtrack].otherID].obstacle,Vector3.zero,Quaternion.identity);
			temp.transform.parent=poolManager.transform;
			poolManager.pObstacle[obstacles[randtrack].otherID].pools.Add(temp);
			temp.SetActive(false);
			return temp;

		}
		return obstacles[randtrack].poolTrack[obstacles[randtrack].poolTrack.Count-1];
	}
	public void doProcedure(Procedures p,GameObject temp,Vector3 rot){
	switch(p.id){
		case 0:
			temp.transform.up=rot;
			break;
		case 1:
			temp.transform.Rotate(p.rot);
			break;
		case 2:
			temp.transform.localPosition+=p.add;
			break;
            case 3:
			float x=Random.Range(p.RandomRotmin.x,p.RandomRotmax.x);
			float y=Random.Range(p.RandomRotmin.y,p.RandomRotmax.y);
			float z=Random.Range(p.RandomRotmin.z,p.RandomRotmax.z);
			temp.transform.Rotate(new Vector3(x,y,z));
			break;
		case 4:
			float x1=Random.Range(p.RandomPosmin.x,p.RandomPosmax.x);
			float y1=Random.Range(p.RandomPosmin.y,p.RandomPosmax.y);
			float z1=Random.Range(p.RandomPosmin.z,p.RandomPosmax.z);
			temp.transform.localPosition+=new Vector3(x1,y1,z1);
			break;
		case 5:
			temp.transform.LookAt(p.rotaxis.position);
			break;
		case 6:
			float x2=Random.Range(p.Randomtransmin.x,p.Randomtransmax.x);
			float y2=Random.Range(p.Randomtransmin.y,p.Randomtransmax.y);
			float z2=Random.Range(p.Randomtransmin.z,p.Randomtransmax.z);
			temp.transform.Translate(x2,y2,z2);
			break;
		case 7:
			temp.transform.Translate(p.TransformVector);
			break;
		case 8:
			temp.transform.rotation=p.rotatevector.rotation;
			
			break;

		};
	}
	//Maths functions section
	int Probability(List<float> probabs, int size){
		float sum=0f;
		for (int i=0; i<size;++i) {
			sum+=probabs[i];
		}
		float value = sum* Random.value;
        if (sum == 0f)
            return -1;
		float sum2=probabs[0];
		for ( int i=0; i< size; ++i){
			if(sum2>value){
				return 0;
			}else if(size-i==1){
				return i;
			}
			else if( value>sum2&&value<sum2+probabs[i+1]){
				return i+1;
			}
			sum2+=probabs[i+1];
		}
		return -1;
		
		
	}
}


[System.Serializable]
public class Obstacles {

		public List<GameObject> poolTrack=new List<GameObject>();
	public GameObject obstacle;
	public int otherID=0; 
	public float probability=1;
	public float sepdist=2;
    public bool ZingVariableObject = false;
    public string Name;
	public TrackObstacle.transenum trans=TrackObstacle.transenum.None;
	public List<Procedures> procedures=new List<Procedures>();
	public bool hasNormals=false;
	public List<bool> showProcedures=new List<bool>();
    public List<Specific_Seperation> SpecificSeperation = new List<Specific_Seperation>();

}
[System.Serializable]
public class Specific_Seperation
{
    public int index;
    public float sepDist = 10f;

}
[System.Serializable]
public class Procedures {
	public int id=0;
	public Vector3 rot;
	public Transform rotaxis;
	public Vector3 add;
	public Transform rotatevector;
	public Vector3 Randomtransmin=Vector3.zero;
	public Vector3 Randomtransmax=Vector3.one; 
	public Vector3 RandomRotmin=Vector3.zero;
	public Vector3 RandomRotmax=Vector3.one;
	public Vector3 TransformVector=Vector3.zero;
	public Vector3 RandomPosmin=Vector3.zero;
	public Vector3 RandomPosmax=Vector3.one;

}
