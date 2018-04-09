using UnityEngine;
using System.Collections;

public class destroyself : MonoBehaviour {
	public float timer;
	public GameObject player;
    public int id = 0;
	public TrackObject t;

	// Use this for initialization
	void Start () {
		player=GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

			
	}
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(MyLoadLevel(timer));
            if (t.UseMultipleRefPoints == true)
                t.destroyOtherTracks(id);
            if (t.LimitSpeed == true)
                t.player.GetComponent<PlayerControls>().isSpeedLimited = false;
        }
	}
	IEnumerator MyLoadLevel(float timer)
	{
		yield return new WaitForSeconds(timer);



		if(player.GetComponent<PlayerControls>().dead==false)
		{
			t.yes=true;
			t.canCreate=false;
		    t.reseed();
			t.gameObject.SetActive(false);
		}


	}
	


	
}
