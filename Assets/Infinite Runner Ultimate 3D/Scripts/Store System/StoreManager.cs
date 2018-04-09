using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class StoreManager : MonoBehaviour {
	public List<StoreObject> Items;
	public bool UpdateUI=false;
	public Text AvaiableMoney;
	public AudioClip BuySound;
	void CheckAllItemsExist(){ 
		for (int i=0; i<Items.Count; i++) {

		if(PlayerPrefs.HasKey(Items[i].ObjectId+"isBought")==false){
				PlayerPrefs.SetInt(Items[i].ObjectId.ToString()+"isBought",0) ;
				Debug.Log("New store item entry created in Prefs");
				if(Items[i].ExhaustibleType)
					PlayerPrefs.SetInt(Items[i].ObjectId.ToString()+"unitsBought",0) ;
			}
		}
	}


	void Start(){
		CheckAllItemsExist ();
      
	}
	void Update(){
		if(UpdateUI==true)
		UpdateStoreUI ();
	}


	void UpdateStoreUI() {
		if (AvaiableMoney)
						AvaiableMoney.text = MoneyAvailable ().ToString();
		for (int i=0; i<Items.Count; ++i) {
			if(Items[i].UI){
			if(	PlayerPrefs.GetInt(Items[i].ObjectId.ToString()+"isBought")==1)
				Items[i].UI.SetActive(false);
			else
				Items[i].UI.SetActive(true);
			}

			if(Items[i].ExhaustibleType){
			if(Items[i].AvailableUnits)
					Items[i].AvailableUnits.text=PlayerPrefs.GetInt(Items[i].ObjectId.ToString()+"unitsBought").ToString();
			}
			   }
	
	}
	public float MoneyAvailable(){
		return PlayerPrefs.GetFloat ("MoneyAvailable");
	}
	public void BuyItem(int ObjectId){
		int index=-1;
		bool GetStatus = GetItemDetailisBought(ObjectId, ref index); //Check whether item is bought

		if (index == -1)
						return;

		if (GetStatus == false) {
			if(Items[index].ExhaustibleType==false){
				if(PlayerPrefs.GetFloat("MoneyAvailable")>=Items[index].price){
					PlayerPrefs.SetInt(Items[index].ObjectId.ToString()+"isBought",1);
					PlayerPrefs.SetFloat("MoneyAvailable",PlayerPrefs.GetFloat("MoneyAvailable")-Items[index].price);
					Debug.Log("Bought an item"+". Available money is "+PlayerPrefs.GetFloat("MoneyAvailable"));
					GetComponent<AudioSource>().PlayOneShot(BuySound);

				}else {
					Debug.Log("Sorry no money");
				}
				//Buy the item
			}else {
				if(PlayerPrefs.GetFloat("MoneyAvailable")>=Items[index].price&&PlayerPrefs.GetInt(Items[index].ObjectId.ToString()+"unitsBought")<Items[index].MaxUnitPurchase)
				{
					PlayerPrefs.SetFloat("MoneyAvailable",PlayerPrefs.GetFloat("MoneyAvailable")-Items[index].price);
					PlayerPrefs.SetInt(Items[index].ObjectId.ToString()+"unitsBought",PlayerPrefs.GetInt(Items[index].ObjectId.ToString()+"unitsBought")+1);
					if(PlayerPrefs.GetInt(Items[index].ObjectId.ToString()+"unitsBought")==Items[index].MaxUnitPurchase)
						PlayerPrefs.SetInt(Items[index].ObjectId.ToString()+"isBought",1);
					Debug.Log("Bought an item"+". Available money is "+PlayerPrefs.GetFloat("MoneyAvailable"));
					GetComponent<AudioSource>().PlayOneShot(BuySound);
				}
			
			}
				
				} else 
			Debug.LogWarning("Attempt to Buy already bought Item!");		


	}

	bool GetItemDetailisBought(int ObjectId,ref int index){ //Get the details of an item of this store
	
		for (int i=0; i<Items.Count; ++i) { //Searching for the item
		if(Items[i].ObjectId==ObjectId){
				index=i;
				break;
		}
	}
		if (index == -1)
						Debug.LogError ("Wrong ObjectId passed as argument");//If not found

		else {
						
								if (PlayerPrefs.GetInt (Items [index].ObjectId.ToString() + "isBought") == 0)
										return false;
								else
										return true;

						
				}

				
		return false;

}
	public bool GetItemDetailisBought(int ObjectId){ //Get the details of an item of this store. Public Function
		int index = -1;
		for (int i=0; i<Items.Count; ++i) { //Searching for the item
			if(Items[i].ObjectId==ObjectId){
				index=i;
				break;
			}
		}
		if (index == -1)
			Debug.LogError ("Wrong ObjectId passed as argument");//If not found
		
		else {

				if (PlayerPrefs.GetInt (Items [index].ObjectId.ToString() + "isBought") == 0)
					return false;
				else
					return true;
				

		}
		
		
		return false;
		
	}
	public int ExhaustibleUnitsBought(int ObjectId){
		int index = -1;
		for (int i=0; i<Items.Count; ++i) { //Searching for the item
			if(Items[i].ObjectId==ObjectId){
				index=i;
				break;
			}

		}
		if (index == -1)
			Debug.LogError ("Wrong ObjectId passed as argument");//If not found
		return PlayerPrefs.GetInt(Items[index].ObjectId.ToString()+"unitsBought");

	}
	public void UseExhaustibleItem(int ObjectId){ //Use an exhaustible Item
		int items = ExhaustibleUnitsBought (ObjectId);
		if (items == 0) {
			Debug.LogWarning ("No items left to buy");
		} else {
			int index=-1;
			bool isBought=GetItemDetailisBought(ObjectId,ref index);

			PlayerPrefs.SetInt(Items[index].ObjectId.ToString()+"unitsBought",PlayerPrefs.GetInt(Items[index].ObjectId.ToString()+"unitsBought")-1);
			if(PlayerPrefs.GetInt(Items[index].ObjectId.ToString()+"unitsBought")<Items[index].MaxUnitPurchase)
				PlayerPrefs.SetInt(Items[index].ObjectId.ToString()+"isBought",0);
		}
	
	}
}

[System.Serializable]

public class StoreObject{
	public string Name;//Name of the Item
	public int ObjectId;//Assign an unique object id to each store item. Changing the id will make the previous purchase obsolete
	public string Description;
	public float price;
	public bool ExhaustibleType; //Is the object exhaustible type that is it can be purchased more than once and used
	public float MaxUnitPurchase;//Max no of times it can be purchased
	bool isBought=false;//Has the object been bought
	public GameObject UI; //Deactivate UI if Bought. Assign the UI Button Here..
	public Text AvailableUnits;
}

