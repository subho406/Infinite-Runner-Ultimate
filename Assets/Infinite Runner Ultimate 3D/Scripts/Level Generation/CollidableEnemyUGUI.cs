/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script is attached to the enemy object


*/

using UnityEngine;
using System.Collections;

public class CollidableEnemyUGUI : MonoBehaviour {
	private GameObject Player;
	private PlayerPoweUpsUGUI pu;

	void Start() {
		Player=GameObject.FindGameObjectWithTag("Player");
		pu=Player.GetComponent<PlayerPoweUpsUGUI>();
		
	}
	void Update() {
		if(pu.CurrentPowerState==PlayerPoweUpsUGUI.State.Invincible||pu.CurrentPowerState==PlayerPoweUpsUGUI.State.FastRun){
			if(gameObject.GetComponent<Collider>().enabled==true)
			gameObject.GetComponent<Collider>().enabled=false;
		}else{
			if(gameObject.GetComponent<Collider>().enabled==false)
				gameObject.GetComponent<Collider>().enabled=true;
		}
	}
}
