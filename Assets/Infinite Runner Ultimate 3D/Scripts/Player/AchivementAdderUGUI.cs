using UnityEngine;
using System.Collections;

public class AchivementAdderUGUI : MonoBehaviour {
	//A Sample script to add new achivements to Acheivement Manager
	public PlayerScoreUGUI playerScore;
	public AchivementsUGUI achivementscript;

	void Update () {
		//Add basic conditions here.
		if(PlayerScoreUGUI.playerDistance() > 500f){
			achivementscript.CompleteAnAchivement(0); //Calling the complete achivement function of Achivements script
		}
		if(PlayerScoreUGUI.playerDistance() > 3000f){
			achivementscript.CompleteAnAchivement(1);
		}
		if(playerScore.playerCoin()>200f){
			achivementscript.CompleteAnAchivement(2);
		}
		if(playerScore.playerCoin()>1000f){
			achivementscript.CompleteAnAchivement(3);
		}
	}
}
