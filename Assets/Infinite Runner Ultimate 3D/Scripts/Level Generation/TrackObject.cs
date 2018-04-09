/* 
Infinite Runner Ultimate Presented by BornFree labs ©
         Programmed by Subhojeet Pramanik

This script signals the World manager to create new track at its next signal ref point, and recycles its track obstacles


*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class TrackObject : MonoBehaviour {

	public Transform target;
	public List<TrackObstacle> TrackObstacles;
    public List<LevelPopulator> LevelPopulators;
    [HideInInspector]
	public bool UseWaypoint=false;
    [HideInInspector]
	public WaypointProgressTracker WaypointProgress;
	public GameObject EnterTrigger;
	[HideInInspector]
	public bool CurrentState=false;
    [HideInInspector]
	public  GameObject player;
	
	public float SignalDistance=50f;
	public List<int> NextTrackException;
	[HideInInspector]
	public bool yes=true;
	public GameObject ExitTrigger;
	public float DestroyTimer=5f;
	public bool HasTurns=false;
	[HideInInspector]
	public bool canCreate=false;
    [HideInInspector]
	public bool LimitSpeed;
    [HideInInspector]
	public float LimitedSpeed=20f;
    [HideInInspector]
    public WorldManager worldmanager;
    [HideInInspector]
    public bool UseMultipleRefPoints = false;
    [HideInInspector]
    public GameObject NextTrackRefPoint;
    [HideInInspector]
    public GameObject[] nextTrackPoints = new GameObject[3];
    [HideInInspector]
    public int numPoints=2;
    [HideInInspector]
    public GameObject[] ExitTriggers = new GameObject[3];
    [HideInInspector]
    public TrackObject[] TrackObjects = new TrackObject[3];
    [HideInInspector]
    public bool hasEnteredCheck = false; //This check is applied when the track is created  by Track with multiple ref point 

    
	public void reseed(){
        for (int i=0; i<TrackObstacles.Count;++i){
			TrackObstacles[i].Recycle();


		}
    for(int i = 0; i < LevelPopulators.Count; i++)
        {
            LevelPopulators[i].RecyleObstacles();
        }
          
	}
 
	void Start () {

		CurrentState=false;
	    EnterTrigger.layer=2;
		
		worldmanager=GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldManager>();
     
		player=GameObject.FindGameObjectWithTag("Player");
        if (UseMultipleRefPoints == false)
        {
            ExitTrigger.layer = 2;
            destroyself d = ExitTrigger.AddComponent<destroyself>();
            d.timer = DestroyTimer;
            d.t = this;
        }
        else
        {
            for (int i = 0; i < numPoints; i++)
            {
                ExitTriggers[i].layer = 2;
                destroyself d = ExitTriggers[i].AddComponent<destroyself>();
                d.timer = DestroyTimer;
                d.t = this;
                d.id = i;
            }
        }
		if(!GameObject.FindGameObjectWithTag("GameManager")){
			Debug.LogError("Could not find the GameManager");
		}
		if(UseWaypoint==true){
			//if(Waypoint.target)
			//target=Waypoint.target;
		}
		if(EnterTrigger) {
		TriggerEnter t=EnterTrigger.AddComponent<TriggerEnter>();
		t.t=this;
     
			t.w=WaypointProgress;
		}


	}
    public void destroyOtherTracks(int exceptionID)
    {
        for (int i = 0; i < numPoints; i++)
        {
            if (i != exceptionID)
            {
                
                TrackObjects[i].yes = true;
                TrackObjects[i].hasEnteredCheck = false;
                TrackObjects[i].canCreate = false;
                TrackObjects[i].reseed();
                TrackObjects[i].gameObject.SetActive(false);
            }
        }
    }

	void Update () {
	if(CurrentState==true){
			worldmanager.CurrentTarget=target;
			CurrentState=false;
			}
		if(yes==true){
			if(HasTurns==true){
				if(canCreate==true){
					if(Vector3.Distance(nextTrackPoints[0].transform.position, player.transform.position)<SignalDistance){
                        if (UseMultipleRefPoints == true)
                        {
                            if (hasEnteredCheck == false)
                            {
                                for (int i = 0; i < numPoints; i++)
                                {
                                    TrackObjects[i] = worldmanager.receiveSignal2(nextTrackPoints[i].transform.position, nextTrackPoints[i].transform.rotation, NextTrackException);
                                    TrackObjects[i].hasEnteredCheck = true;
                                }
                                yes = false;
                            }
                        }
                        else
                        {
                            if (hasEnteredCheck ==false)
                            {
                                worldmanager.receiveSignal(NextTrackRefPoint.transform.position, NextTrackRefPoint.transform.rotation, NextTrackException);
                                yes = false;
                            }
                        }

				}
			}
			}else{
	if(Vector3.Distance(NextTrackRefPoint.transform.position,player.transform.position)<SignalDistance){
        if (UseMultipleRefPoints == true)
        {
            if (hasEnteredCheck == false)
            {
                for (int i = 0; i < numPoints; i++)
                {
                    TrackObjects[i] = worldmanager.receiveSignal2(nextTrackPoints[i].transform.position, nextTrackPoints[i].transform.rotation, NextTrackException);
                    TrackObjects[i].hasEnteredCheck = true;
                }
                yes = false;
            }
        }
        else
        {
            if (hasEnteredCheck == false)
            {
                worldmanager.receiveSignal(NextTrackRefPoint.transform.position, NextTrackRefPoint.transform.rotation, NextTrackException);
                yes = false;
            }
        }
		

		
				}
			}
		}
}
}
