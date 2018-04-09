
/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script Manages the GUI
A sample script on carrying basic functions like saving and loading of scores, updating the main menu GUI , changing graphics and volume,

*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GUIManagerUGUI : MonoBehaviour {
    [HideInInspector]
	public bool GameStarted=false; //Whether Game has started
	public GameObject PlayerCamera; //The main Player Camera
	GameObject Player;
	PlayerControls p; //The Main Player PlayerControls
	bool movedtopos; //Moved to the end of transition position
	public GameObject MainMenuCamera; //The main camera in main menu
	public GameObject MainMenuGUI; //The GUI camera in main menu
	public GameObject PlayTimeGUI; //The Play time GUI camera
	private bool startPlayTransition;
	public float PlayTransitionSpeed=5f;
	public AudioSource Music;  //The music AudioSource

	public Toggle ControlsTypeToggle; //Toggle that changes Player control in settings
    public GameObject PauseGUI;   //The Pause and score GUI parent
	public GameObject PauseMenuGUI;  //The Pause menu GUI parent
	public Slider MusicSlider;
	public Slider GraphicsSlider;
	public Toggle Fog;
	public GameObject DeathGUI;
	public Text HighScoreCoin;
	public Text HighScoreScore;
	public Text HighScoreDistance;
    public StoreManager Store;
    public GameObject RevivalButton; //This GameObject is deacativated if revival is not bought in store.
    public Text RevivalText;//The text that displays number of revivals available

	void Start () {
		startPlayTransition = false;
		GameStarted=false;
		Player=GameObject.FindGameObjectWithTag("Player");
		p=Player.GetComponent<PlayerControls>();
		movedtopos=false;
		if(PlayerPrefs.GetFloat("FirstTime")==0){ //If this is the first time the game is opened
			GamePreferencesCreate(); //Create the preferences
		}else {
			GamePreferencesReload(); //Else reload the saved preferences
		}

		HighScoreCoin.text=PlayerPrefs.GetFloat("Coin").ToString();  
		HighScoreScore.text=PlayerPrefs.GetFloat("Score").ToString();
		//HighScoreDistance.text=PlayerPrefs.GetFloat("Distance").ToString();
	}
	
	// Update is called once per frame
	public void hitMainMenuPlay(){
		startPlayTransition = true;
	}
	public void hitGamePlayPause(){
		if (GameStarted == true) {
						p.CurrentGameState = PlayerControls.GameState.Pause;
						PauseGUI.SetActive (false);
						PauseMenuGUI.SetActive (true);
				}
	}
	public void hitGamePlayResume(){
		p.CurrentGameState=PlayerControls.GameState.Playing;
		PauseGUI.SetActive(true);
		PauseMenuGUI.SetActive(false);
	}
	public void hitGamePlayMainMenu(){
		Application.LoadLevel(0);
	}
	public void hitMainMenuExit(){
		Application.Quit();
		}
	void Update () {
        if (Store)   //Sample store codes to check whether item is bought and if so carrying out desired actions
        {
            if (Store.ExhaustibleUnitsBought(100) > 0)//Check if revival is bought in store. 100 is the item id for revival in store. It is an exhaustible item
            {
                RevivalButton.SetActive(true); //If bought activate revival button. Revival buttton is shown when player dies
                RevivalText.text = Store.ExhaustibleUnitsBought(100).ToString() + " revivals available";


            }
            else
            {
                RevivalButton.SetActive(false);
            }
        }
		if (startPlayTransition == true) {//Start Play transition animation
						if (GameStarted == false) {
								MainMenuCamera.transform.parent = null;
								MainMenuCamera.transform.position = Vector3.MoveTowards (MainMenuCamera.transform.position, PlayerCamera.transform.position, PlayTransitionSpeed * Time.deltaTime);//Camera move animation
								MainMenuCamera.transform.rotation = Quaternion.Slerp (MainMenuCamera.transform.rotation, PlayerCamera.transform.rotation, PlayTransitionSpeed * Time.deltaTime);
								MainMenuGUI.SetActive (false);
				
								if (Vector3.Distance (MainMenuCamera.transform.position, PlayerCamera.transform.position) < 0.1f) {
					
										Destroy (MainMenuCamera);
										PlayerCamera.SetActive (true);
										p.CurrentGameState = PlayerControls.GameState.Playing;		
										GameStarted = true;
										PlayTimeGUI.SetActive (true);
										startPlayTransition = false;
								}
						}
				}	
		
			if(ControlsTypeToggle.isOn==true){//Check change of controls in settings
				p.TrackType=PlayerControls.TrackTypeEnum.ThreeSlotTrack;
			PlayerPrefs.SetInt ("ControlType", 1);
			}else {
				p.TrackType=PlayerControls.TrackTypeEnum.FreeHorizontalMovement;
			PlayerPrefs.SetInt ("ControlType", 0);
			}
		//Graphics changing code
		if (Fog.isOn == true) {
						RenderSettings.fog = true;
						PlayerPrefs.SetInt ("Fog", 1);
				} else {
			RenderSettings.fog=false;
			PlayerPrefs.SetInt("Fog",0);
		}

		PlayerPrefs.SetFloat("Graphics",GraphicsSlider.value);
		if(GraphicsSlider.value==0)
			QualitySettings.SetQualityLevel(1);
		
		else if(GraphicsSlider.value==1)
			QualitySettings.SetQualityLevel(2);
		else if(GraphicsSlider.value==2)
			QualitySettings.SetQualityLevel(3);
		else if(GraphicsSlider.value==3)
			QualitySettings.SetQualityLevel(4);
		else if(GraphicsSlider.value==4)
			QualitySettings.SetQualityLevel(5);
		else
			QualitySettings.SetQualityLevel(6);
		//end Graphics changing code
		Music.volume=MusicSlider.value;
		PlayerPrefs.SetFloat("Music",MusicSlider.value);


		if(GameStarted==true&&p.CurrentGameState==PlayerControls.GameState.Dead){
			PauseGUI.SetActive(false);
			DeathGUI.SetActive(true);

		}
        if (GameStarted == true && p.CurrentGameState == PlayerControls.GameState.Playing)
        {
            if(PauseGUI.activeInHierarchy==false)
                PauseGUI.SetActive(true);
        }
	}



	void GamePreferencesReload() {
		if (PlayerPrefs.GetInt ("ControlType") == 1) {
						p.TrackType = PlayerControls.TrackTypeEnum.ThreeSlotTrack;
						ControlsTypeToggle.isOn = true;
				} else {
			p.TrackType = PlayerControls.TrackTypeEnum.FreeHorizontalMovement;
			ControlsTypeToggle.isOn = false;
		}
		float value=PlayerPrefs.GetFloat("Graphics");
		GraphicsSlider.value = value;
		if(value==0)
			QualitySettings.SetQualityLevel(1);
		
		else if(value==1)
			QualitySettings.SetQualityLevel(2);
		else if(value==2)
			QualitySettings.SetQualityLevel(3);
		else if(value==3)
			QualitySettings.SetQualityLevel(4);
		else if(value==4)
			QualitySettings.SetQualityLevel(5);
		else
			QualitySettings.SetQualityLevel(6);
	
	   if(PlayerPrefs.GetInt("Fog")==1){
			RenderSettings.fog=true;
			Fog.isOn=true;

		}
		else{
			RenderSettings.fog=false;
			Fog.isOn=false;

		}
	
		Music.volume=PlayerPrefs.GetFloat("Music");
		MusicSlider.value=Music.volume;


	}
	void GamePreferencesCreate(){ //save default Game Preferences. Change default preferences from here
		PlayerPrefs.SetFloat("Graphics",0.4f);
		PlayerPrefs.SetFloat("Music",1f);
		PlayerPrefs.SetInt("Fog",0);
		PlayerPrefs.SetInt("Effects",0);
		PlayerPrefs.SetFloat("FirstTime",1);
		PlayerPrefs.SetInt ("ControlType", 0);//0 stands for free movement and 1 stands for swipe based similar to subway surfer
	}


}
