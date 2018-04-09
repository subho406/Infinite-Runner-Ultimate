/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script manages the player Achivements or objectives and writes them to PlayerPrefs and changes the title of UILabel of NGUI


*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class AchivementsUGUI : MonoBehaviour {
	public List<Achivement> achivements;


	void Start () {
		init();

	}
	

	public void CompleteAnAchivement (int id) { //This function has to be called from other script to complete an achivement. mention the achivement id as argument. For demo checks have been added to PlayerScore.cs which calls this function whenever condition is met
	if(id>achivements.Count)
			Debug.Log("unavailable Achivement");
	else {
			PlayerPrefs.SetInt(achivements[id].Title+id,1); 
		}
	}
	void init() {
		for(int i=0; i<achivements.Count;++i){
		if(PlayerPrefs.GetInt(achivements[i].Title+i)==1){
				achivements[i].CheckMarkImage.gameObject.SetActive(true);
			}else{
				achivements[i].CheckMarkImage.gameObject.SetActive(false);
			}
		}
	}
}

[System.Serializable]
public class Achivement {
	public string Title;
	public Image CheckMarkImage;
	}