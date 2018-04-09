using UnityEngine;
using System.Collections;

public class PlayerPoweUpsUGUI : MonoBehaviour {
	public enum State {
		None,
		Invincible,
		CoinMagnet,
		FastRun
	};
	private GameObject Player;
	private PlayerControls pc;
	private PlayerScoreUGUI ps;
	public State CurrentPowerState;
	public float InvincibleDuration;
	public float InvincibleScore;
	public GameObject InvincibleParticles;
	public float FastRunDuration;
	public float FastRunSpeed;
	public float FastRunScore;
	public float CoinMagnetDuration;
	public GameObject CoinMagnetParticle;
	public float CoinMagnetScore;
	public AudioClip PowerUpSound;
	private float timer=0f;
	private State prevState;
	float t;
	float mt;
	float mtimer=0f;
	private bool Secondpoweractive=false;
	private float CachedSpeed;
	float AnimateInterval=0.1f;
	float animatetimer=0f;
	void Start(){
		Player=GameObject.FindGameObjectWithTag("Player");
		pc=Player.GetComponent<PlayerControls>();
		ps=Player.GetComponent<PlayerScoreUGUI>();

	}
	void Update () {
		if(pc.CurrentGameState==PlayerControls.GameState.Playing){
	if(CurrentPowerState!=State.None){
			timer+=Time.deltaTime;
				if(timer>t-2f&&(CurrentPowerState==State.FastRun||CurrentPowerState==State.Invincible)){
					animatetimer+=Time.deltaTime;
					if(animatetimer>AnimateInterval){
						animatetimer=0f;
						if(InvincibleParticles.activeInHierarchy==true)
							InvincibleParticles.SetActive(false);
						else
							InvincibleParticles.SetActive(true);
					}
				}
					if(timer>t){
					CurrentPowerState=State.None;
					DeactivatePrevState();
					}


			}


		}
	}
	public void ActivateState(State state){
	if(pc.CurrentGameState==PlayerControls.GameState.Playing&&CurrentPowerState==State.None){
			DeactivatePrevState();

			timer=0f;
			CurrentPowerState=state;
			switch(CurrentPowerState){
			case State.CoinMagnet:
				t=CoinMagnetDuration;
				CoinMagnetActivate();
				prevState=State.CoinMagnet;
				break;
			case State.FastRun:
				t=FastRunDuration;
				FastRunActivate();
				prevState=State.FastRun;
				break;
			case State.Invincible:
				InvincibilityActivate();
				t=InvincibleDuration;
				prevState=State.Invincible;
				break;

			};
			GetComponent<AudioSource>().PlayOneShot(PowerUpSound);
		}
	}
	void DeactivatePrevState() {
		switch(prevState){
		case State.CoinMagnet:
			CoinMagnetDeactivate();
			break;
		case State.Invincible:
			InvincibilityDeactivate();
			break;
		case State.FastRun:
			FastRunDeactivate();
			break;
		};
	}
	void CoinMagnetActivate() {
		CoinMagnetParticle.SetActive(true);
		ps.addScore(CoinMagnetScore);
	}
	void CoinMagnetDeactivate() {
		CoinMagnetParticle.SetActive(false);
	}
	void InvincibilityActivate() {
		ps.addScore(InvincibleScore);
		InvincibleParticles.SetActive(true);

	}
	void InvincibilityDeactivate(){
		InvincibleParticles.SetActive(false);
	}
	void FastRunActivate(){
		ps.addScore(FastRunScore);
		pc.AutoTurn=true;
		pc.FastRun=true;
		CachedSpeed=pc.TargetSpeed;
		pc.TargetSpeed=FastRunSpeed;
		InvincibleParticles.SetActive(true);
	}
	void FastRunDeactivate(){
		pc.TargetSpeed=CachedSpeed;
		pc.FastRun=false;
		pc.AutoTurn=false;
		InvincibleParticles.SetActive(false);
	}
}
