/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script manages the Player Score, Coins and distance


*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerScoreUGUI : MonoBehaviour {
	private float CurrentPlayerScore;
	public Text PlayerScoreGUI;
	private float CurrentPlayerCoin;
	public Text PlayerCoinGUI;
	public AudioClip CoinCollectSound;
	public ParticleEmitter CoinCollectParticles;
	private GameObject player;
	private PlayerControls pc;
	public GameObject DistanceGUIObject; //The object containing the distance ui text
	public Text DistanceGUI;
	private static float Distance=0f;
	private float UpdateDistance=500f;
	private float CoinParticleCounter=0f;
	private bool showCoinParticle=false;
	private bool hasSetMoney=false;
	public float playerCoin(){
		return CurrentPlayerCoin;
	}
	public float addScore(float score) { //Function to add score to prefs
		CurrentPlayerScore+=score;
		return 0;
	}

	public float playerScore(){
		return CurrentPlayerScore;
	}
	public static float playerDistance() {
		return Distance;
	}
	public float addCoin(float coin) {
		CurrentPlayerCoin+=coin;
		GetComponent<AudioSource>().PlayOneShot(CoinCollectSound,1f);
		showCoinParticle=true;
		CoinParticleCounter=0f;
		return 0;

	}
	// Use this for initialization
	void Start () {
		hasSetMoney = false;
		CoinParticleCounter=0f;
		Distance=0f;
		UpdateDistance=500f;
		player=GameObject.FindGameObjectWithTag("Player");
		pc=player.GetComponent<PlayerControls>();
		CurrentPlayerScore=0f;
		CurrentPlayerCoin=0f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
  
        if(showCoinParticle==true){
		if(CoinCollectParticles.emit==false)
				CoinCollectParticles.emit=true;
			CoinParticleCounter+=Time.deltaTime;
			if(CoinParticleCounter>0.6f){
				showCoinParticle=false;
				CoinCollectParticles.emit=false;
			}
		}
		PlayerCoinGUI.text=CurrentPlayerCoin.ToString();
		PlayerScoreGUI.text=CurrentPlayerScore.ToString();
		if(pc.CurrentGameState==PlayerControls.GameState.Playing){
			CurrentPlayerScore+=1f;
			Distance+=pc.speed*Time.deltaTime;
			if(Distance>UpdateDistance){
				UpdateDistance+=500f;
				StartCoroutine("DistanceShow");
			}

		}
		if(pc.CurrentGameState==PlayerControls.GameState.Dead){ //Save score to prefs after death 
			if(hasSetMoney==false){
			float money=PlayerPrefs.GetFloat("MoneyAvailable")+CurrentPlayerCoin;
				hasSetMoney=true;
				PlayerPrefs.SetFloat("MoneyAvailable",money);
			}

			if(CurrentPlayerScore>PlayerPrefs.GetFloat("Score"))
				PlayerPrefs.SetFloat("Score",CurrentPlayerScore);

			if(CurrentPlayerCoin>PlayerPrefs.GetFloat("Coin"))
				PlayerPrefs.SetFloat("Coin",CurrentPlayerCoin);
			if(Distance>PlayerPrefs.GetFloat("Distance"))
				PlayerPrefs.SetFloat("Distance",(int)Distance);

		}
	}
	IEnumerator DistanceShow() { //Activate the distance GUI shown after every 500m 
		DistanceGUIObject.SetActive(true);
		float elapsed = 0.0f;
		while (elapsed < 3f) {
			float f=UpdateDistance-500f;
			DistanceGUI.text=f.ToString()+" m";
			elapsed += Time.deltaTime; 
			yield return null;
		
		}
		DistanceGUIObject.SetActive(false);
	}
}
