/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script manages the pools of the props and enemies


*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
[AddComponentMenu("Infinite Runner Tool/Pool Manager")]
public class PropPoolManager : MonoBehaviour {
	public List<poolObstacle> pObstacle=new List<poolObstacle>();
	public List<string> Names=new List<string>();
	public List<bool> foldout=new List<bool>();
	void Start () {
		for(int i=0; i<pObstacle.Count; ++i){
			for(int j=0; j<pObstacle[i].pooledAmount;++j){

				pObstacle[i].pools.Add((GameObject)Instantiate(pObstacle[i].obstacle,Vector3.zero, Quaternion.identity));
				pObstacle[i].pools[j].transform.parent=transform;
				pObstacle[i].pools[j].SetActive(false);
			}
		}
	}
	

}

[System.Serializable]
public class poolObstacle {
	public GameObject obstacle;
	public List<GameObject> pools=new List<GameObject>();
	public float pooledAmount=5f;

}
