using UnityEngine;
using System.Collections;

public class ROtate : MonoBehaviour {
	float timer=0f;
	float sign=-1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate(sign*Vector3.forward*30*Time.deltaTime);
	}
}
