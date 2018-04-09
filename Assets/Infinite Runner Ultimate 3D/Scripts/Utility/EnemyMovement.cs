using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {
	private GameObject Player;
	public float SignalDistance;
	public float Speed;
	// Use this for initialization
	void Start () {
		Player=GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
if(Vector3.Distance(transform.position,Player.transform.position)<SignalDistance){

			transform.Translate(Vector3.forward*Speed*Time.deltaTime);

		}
	}
}
