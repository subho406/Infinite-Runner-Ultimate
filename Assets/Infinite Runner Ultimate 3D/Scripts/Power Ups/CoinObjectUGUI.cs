using UnityEngine;
using System.Collections;

public class CoinObjectUGUI : MonoBehaviour {
	private GameObject Player;
	private PlayerScoreUGUI playerScore;
	private PlayerControls pc;
	public float CoinValue=1f;
	private PlayerPoweUpsUGUI pu;
	public float ScoreValue=10f;
	public bool GroupCoin=false;
	private Vector3 localPos;
	private bool needtorest=false;
	private float ResetTimer=0f;
	public float CoinMagnetRadius;

	void Start() {
		Player=GameObject.FindGameObjectWithTag("Player");
		playerScore=Player.GetComponent<PlayerScoreUGUI>();
		pu=Player.GetComponent<PlayerPoweUpsUGUI>();
		localPos=transform.localPosition;
		pc=Player.GetComponent<PlayerControls>();

	}
	void OnTriggerEnter(Collider other) {
	if(other.gameObject.tag=="Player"){
			playerScore.addCoin(CoinValue);
			playerScore.addScore(ScoreValue);
			transform.position-=new Vector3(0,CoinMagnetRadius+1,0);  //Hiding the coin away from view as if it was destroyed. We can't recycle it now because the TrackObstacle must do it when the Track is recycled.Increase the value if you can still see the coin.
			if(GroupCoin){
				needtorest=true;
				ResetTimer=0f;
			}
		}
	}
	void Update () {
	if(needtorest){
			ResetTimer+=Time.deltaTime;
			if(ResetTimer>4f){
				transform.localPosition=localPos;
				needtorest=false;
			}
		}
		if(pu.CurrentPowerState==PlayerPoweUpsUGUI.State.CoinMagnet){
		if(Vector3.Distance(Player.transform.position,transform.position)<CoinMagnetRadius){
				transform.position=Vector3.MoveTowards(transform.position,Player.transform.position,pc.speed*Time.deltaTime);
				needtorest=true;
				ResetTimer=0f;

			}
		}
	}

}
