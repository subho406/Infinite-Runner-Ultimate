/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script changes the values on Curve shader


*/

using UnityEngine;
using System.Collections;

public class CurveManager : MonoBehaviour {
	public Vector4 QOffsetMin;
	public Vector4 QOffsetMax;
	public float Distance;
	public float ChangeFrequencyMax=15f;
	public float ChangeFrequencyMin=5f;
	private float timer=0f;
	private float currentchangefreq;
	private Vector4 currentOffset;
	private Vector4 nextOffset;
	public float speed=1f;
	private GameObject Player;
	private PlayerControls p;
	// Use this for initialization
	void Start () {
		currentchangefreq=Random.Range(ChangeFrequencyMin,ChangeFrequencyMax);
		currentOffset=new Vector4(Random.Range(QOffsetMin.x,QOffsetMax.x),Random.Range(QOffsetMin.y,QOffsetMax.y),Random.Range(QOffsetMin.z,QOffsetMax.z),Random.Range(QOffsetMin.w,QOffsetMax.w));
		Player=GameObject.FindGameObjectWithTag("Player");
		p=Player.GetComponent<PlayerControls>();

	}
	
	// Update is called once per frame
	void Update () {
		if(p.CurrentGameState==PlayerControls.GameState.Playing){
		if(p.dead==false){
		if(timer<currentchangefreq){
			timer+=Time.deltaTime;
		}else {
			timer=0f;
			currentchangefreq=Random.Range(ChangeFrequencyMin,ChangeFrequencyMax);
			nextOffset=new Vector4(Random.Range(QOffsetMin.x,QOffsetMax.x),Random.Range(QOffsetMin.y,QOffsetMax.y),Random.Range(QOffsetMin.z,QOffsetMax.z),Random.Range(QOffsetMin.w,QOffsetMax.w));
		}
		currentOffset=Vector4.Lerp(currentOffset,nextOffset,speed*Time.deltaTime);
		Shader.SetGlobalFloat("_Dist",Distance);
		Shader.SetGlobalVector("_QOffset",currentOffset);
	}
		}
	}
}
