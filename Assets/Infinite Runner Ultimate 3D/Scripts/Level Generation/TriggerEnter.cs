using UnityEngine;
using System.Collections;

public class TriggerEnter : MonoBehaviour {
	[HideInInspector]
	public TrackObject t;
	public WaypointProgressTracker w;
   
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag=="Player"){
            if(t.hasEnteredCheck==true)
            t.hasEnteredCheck = false;
		t.CurrentState=true;
			if(t.UseWaypoint==true){
				//Debug.Log("enter trigger");
				t.WaypointProgress.Reset();
				 
			}
			if(t.LimitSpeed==true){
				t.player.GetComponent<PlayerControls>().isSpeedLimited=true;
				t.player.GetComponent<PlayerControls>().LimitedSpeed=t.LimitedSpeed;
			}
		}
	}
	void OnTriggerExit(Collider other) {
				if (other.gameObject.tag == "Player") {
			if(t.LimitSpeed==true){
				t.player.GetComponent<PlayerControls>().isSpeedLimited=true;
				t.player.GetComponent<PlayerControls>().LimitedSpeed=t.LimitedSpeed;
			}
				}
		}
}
