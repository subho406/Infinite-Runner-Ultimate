using UnityEngine;
using System.Collections;

public class PowerUpObjectUGUI : MonoBehaviour {
	private GameObject Player;
	private PlayerPoweUpsUGUI pu;
	public PlayerPoweUpsUGUI.State PowerUpType;
	void Start() {
		Player=GameObject.FindGameObjectWithTag("Player");
		pu=Player.GetComponent<PlayerPoweUpsUGUI>();

	}
	void OnTriggerEnter(Collider other){
	if(other.tag=="Player"){
			pu.ActivateState(PowerUpType);
			transform.position-=new Vector3(0,5f,0);  //Hiding the coin away from view as if it was destroyed. We can't recycle it now because the TrackObstacle must do it when the Track is recycled.Increase the value if you can still see the coin.
		}
	}
}
