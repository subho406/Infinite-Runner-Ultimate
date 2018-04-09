/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script manages the Player Turn Trigger and sends signal to its Track Object. This script does not need to be added it is added automatically


*/

using UnityEngine;
using System.Collections;

public class PlayerTurnTrigger : MonoBehaviour {
    public TrackObject TrackObjectScript;
	
   
   public bool UseCustomTargets=false;

    public Transform LeftTarget;
    public Transform RightTarget;
	public float CreateNextTrackDistanceCheck=10f;
	public enum Type{
	Both,
		Right,
		Left
	};
	public Type type=Type.Left;
	bool canrotate=false;
	GameObject player;
	void Start() {
		gameObject.layer=2;
		player=GameObject.FindGameObjectWithTag("Player");
	}
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag=="Player"){
		canrotate=true;
			player.GetComponent<PlayerControls>().canChangeSlot=false;
		}
		}
	void OnTriggerExit(Collider other){
		if(other.gameObject.tag=="Player"){
		canrotate=false;
			player.GetComponent<PlayerControls>().canChangeSlot=true;
		}
	}
	void Update(){
		if(Vector3.Distance(player.transform.position,transform.position)<CreateNextTrackDistanceCheck){
			TrackObjectScript.canCreate=true;
		}
		if(canrotate==true){

			switch(type){ //Call the rotate if inside the trigger zone
			case Type.Both:
                    
				if(UseCustomTargets){
                    
			player.GetComponent<PlayerControls>().Rotate(transform.position,ref canrotate,0,LeftTarget,RightTarget);
				}else{
					Debug.LogError("Both sided turn cannot work without custom targets. Check use custom targets in PlayerTurnTrigger and assign left and right targets.");
				}
				break;
			case Type.Right:
				if(UseCustomTargets){
					player.GetComponent<PlayerControls>().Rotate(transform.position,ref canrotate,1,RightTarget);
				}else{
					player.GetComponent<PlayerControls>().Rotate(transform.position,ref canrotate,1,TrackObjectScript.target);
				}
				break;
			case Type.Left:
				if(UseCustomTargets){
					player.GetComponent<PlayerControls>().Rotate(transform.position,ref canrotate,-1,LeftTarget);
				}else{
					player.GetComponent<PlayerControls>().Rotate(transform.position,ref canrotate,-1,TrackObjectScript.target);
				}
				break;
			}
			}
	}
}
