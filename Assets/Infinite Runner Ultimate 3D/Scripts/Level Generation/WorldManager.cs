/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script manages the placing of tracks one after the other and recycling them
The Player moves instead of the world which gives great performance but on high distances all positions are again resetted to origin avoiding
all floating point errors.

*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[AddComponentMenu("Infinite Runner Tool/World Manager")]
[System.Serializable]
public class WorldManager : MonoBehaviour{
	private GameObject player;
	public List<World> Worlds=new List<World>();
	public List<bool> showWorld=new List<bool>();
	public int startWorld=0;
	public int PoolAmount=5;
	public Transform CurrentTarget;
	public Transform StartTarget;
	public int currentWorld;
	public int Tracklength;
	public int prevworld;
    public TrackObject currentTrack;
    private PlayerScoreUGUI playerScoreUGUI;
	public int Trackcount=0;
    public bool TwoD = false;
	void Start(){
        
            player = GameObject.FindGameObjectWithTag("Player");
            if (player.GetComponent<PlayerPoweUpsUGUI>())
            {
                playerScoreUGUI = player.GetComponent<PlayerScoreUGUI>();
            }
            for (int i = 0; i < Worlds.Count; ++i)
            {
                Worlds[i].cacheProbability = Worlds[i].probability;
            }
		CurrentTarget=StartTarget;
		//Reseting everything and pooling Tracks
		currentWorld=startWorld;
		prevworld=currentWorld;
		Tracklength=Random.Range (Worlds[currentWorld].minTracklength,Worlds[currentWorld].maxTracklength);

		for(int i=0; i<Worlds.Count; ++i){
            if (Worlds[i].minAppearLimit > 0)
            {
                Worlds[i].probability = 0f;
            }
			for(int j=0; j<Worlds[i].tracks.Count; ++j){
				for(int k=0; k<PoolAmount;++k){
					Worlds[i].tracks[j].pools.Add((GameObject)Instantiate(Worlds[i].tracks[j].track,Vector3.zero,Quaternion.identity));
					Worlds[i].tracks[j].pools[k].SetActive(false);
				}
			}
		}
	}

	/// <summary>
    /// 
    /// </summary>
    void Update (){


            for (int i = 0; i < Worlds.Count; i++)
            {
                if (Worlds[i].minAppearLimit > 0f && Worlds[i].maxAppearLimit != -1f)
                {
                    if (Worlds[i].minAppearLimit > Worlds[i].maxAppearLimit)
                    {
                        Debug.LogError("Appears after distance has greater value than Doesn't appear after distance which is not allowed.");
                        break;
                    }
                }
                if (Worlds[i].minAppearLimit > 0)
                {
                    if (PlayerScoreUGUI.playerDistance() < Worlds[i].minAppearLimit)
                    {
                        Worlds[i].probability = 0f;
                    }
                    else
                    {
                        Worlds[i].probability = Worlds[i].cacheProbability;
                    }
                }
                if (Worlds[i].maxAppearLimit != -1f && PlayerScoreUGUI.playerDistance() > Worlds[i].minAppearLimit) 
                {
                    if (PlayerScoreUGUI.playerDistance() > Worlds[i].maxAppearLimit)
                    {
                        Worlds[i].probability = 0f;
                    }
                    else
                    {
                        Worlds[i].probability = Worlds[i].cacheProbability;
                    }
                }
            }
           
            //Here goes the floating point fix Logic we move the Player along with the world to origin in such a way that no extra relative motion is noticiable
            transform.position = player.transform.position;
		if(Mathf.Abs(transform.position.x)>9000f||Mathf.Abs(transform.position.y)>9000f||Mathf.Abs(transform.position.z)>9000f){ //A value above which move back to origin
			player.transform.parent=this.transform;
			for(int i=0;i<Worlds.Count;++i){
				for(int j=0;j<Worlds[i].tracks.Count;++j){
					for(int k=0; k<Worlds[i].tracks[j].pools.Count;++k){
					if(Worlds[i].tracks[j].pools[k].activeInHierarchy==true){
							Worlds[i].tracks[j].pools[k].transform.parent=this.transform;//Parenting all active tracks to this transform
						}
					}
				}
			}
			transform.position=Vector3.zero;//Moving to origin
			player.transform.parent=null;
			for(int i=0;i<Worlds.Count;++i){
				for(int j=0;j<Worlds[i].tracks.Count;++j){
					for(int k=0; k<Worlds[i].tracks[j].pools.Count;++k){
						if(Worlds[i].tracks[j].pools[k].activeInHierarchy==true){
							Worlds[i].tracks[j].pools[k].transform.parent=null;//Reseting all finally
						}
					}
				}
			}

		}

	}

	void SeedWorld(Vector3 position, Quaternion rotation,List<int> exceptions){

		List<float> probabs=new List<float>();
		for(int i=0; i<Worlds.Count; ++i){
			probabs.Add(Worlds[i].probability);
		}
		currentWorld=Probability(probabs,Worlds.Count);

		Tracklength=Random.Range (Worlds[currentWorld].minTracklength,Worlds[currentWorld].maxTracklength);

		Trackcount=0;
		GameObject pooledObject;
		probabs.Clear();
		for(int i=0; i<Worlds[currentWorld].tracks.Count;++i){
			
			probabs.Add(Worlds[currentWorld].tracks[i].probability);
			
		}
		if(prevworld==currentWorld){
		for(int j=0; j<exceptions.Count;++j){
			
			probabs[exceptions[j]]=0f;
			
			
		}
		}
        if(prevworld!=currentWorld){
		pooledObject=CheckPool(currentWorld,0);
			prevworld=currentWorld;
		}else {
			int seedTrack=Probability(probabs,Worlds[currentWorld].tracks.Count);
			pooledObject=CheckPool(currentWorld,seedTrack);
			prevworld=currentWorld;
		}
		pooledObject.SetActive(true);
		pooledObject.transform.position=position;
		pooledObject.transform.rotation=rotation;
        TrackObject t = pooledObject.GetComponent<TrackObject>();
        if (t){
            
			for(int i=0; i<t.TrackObstacles.Count;++i){
				t.TrackObstacles[i].seedobstacle();
               
			}
            for(int i = 0; i < t.LevelPopulators.Count; i++)
            {

                t.LevelPopulators[i].PopulateObstacles();
   
            }
		}

	}
	//Receive Signal from Refrence object and create Track
	public void receiveSignal(Vector3 position, Quaternion rotation,List<int> exceptions){
	if(Trackcount>=Tracklength){
			SeedWorld(position,rotation,exceptions);
				return ;
		}
		List<float> probabs=new List<float>();
		for(int i=0; i<Worlds[currentWorld].tracks.Count;++i){

			probabs.Add(Worlds[currentWorld].tracks[i].probability);

		}
		for(int j=0; j<exceptions.Count;++j){
			
			probabs[exceptions[j]]=0f;

			
		}

	    int seedTrack=Probability(probabs,Worlds[currentWorld].tracks.Count);


		GameObject pooledObject=CheckPool(currentWorld,seedTrack);

		pooledObject.transform.position=position;
		pooledObject.transform.rotation=rotation;
		pooledObject.SetActive(true);
		if(pooledObject.GetComponent<TrackObject>()){

            TrackObject to = pooledObject.GetComponent<TrackObject>();
            if (!currentTrack)
                currentTrack = to;
            for (int i=0; i<to.TrackObstacles.Count;++i){
				to.TrackObstacles[i].seedobstacle();
               
            }
            for (int i = 0; i < to.LevelPopulators.Count; i++)
            {
                
                to.LevelPopulators[i].PopulateObstacles();
            }
        }
		++Trackcount;
	}
    public TrackObject receiveSignal2(Vector3 position, Quaternion rotation, List<int> exceptions)
    {

        List<float> probabs = new List<float>();
        for (int i = 0; i < Worlds[currentWorld].tracks.Count; ++i)
        {

            probabs.Add(Worlds[currentWorld].tracks[i].probability);

        }
        for (int j = 0; j < exceptions.Count; ++j)
        {

            probabs[exceptions[j]] = 0f;


        }

        int seedTrack = Probability(probabs, Worlds[currentWorld].tracks.Count);


        GameObject pooledObject = CheckPool(currentWorld, seedTrack);

        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;
        pooledObject.SetActive(true);
        if (pooledObject.GetComponent<TrackObject>())
        {
            TrackObject to = pooledObject.GetComponent<TrackObject>();
            if (!currentTrack)
                currentTrack = to;
            for (int i = 0; i < to.TrackObstacles.Count; ++i)
            {
                to.TrackObstacles[i].seedobstacle();
            }

                   }
        ++Trackcount;
        return pooledObject.GetComponent<TrackObject>();
    }

	public GameObject CheckPool (int currentworld,int seedtrack) {
		for(int i=0; i<Worlds[currentworld].tracks[seedtrack].pools.Count;++i){
		if(Worlds[currentworld].tracks[seedtrack].pools[i].activeInHierarchy==false){
				return Worlds[currentworld].tracks[seedtrack].pools[i];
			}
			   }
		Worlds[currentworld].tracks[seedtrack].pools.Add((GameObject)Instantiate(Worlds[currentworld].tracks[seedtrack].track,Vector3.zero,Quaternion.identity));
		Worlds[currentworld].tracks[seedtrack].pools[Worlds[currentworld].tracks[seedtrack].pools.Count-1].SetActive(false);
		return Worlds[currentworld].tracks[seedtrack].pools[Worlds[currentworld].tracks[seedtrack].pools.Count-1];
	}



	//Maths functions sections.
	int Probability(List<float> probabs, int size){
		float sum=0f;
		for (int i=0; i<size;++i) {
			sum+=probabs[i];
		}
		float value = sum* Random.value;

		float sum2=probabs[0];
		for ( int i=0; i< size; ++i){
		if(sum2>value){
				return 0;
			}else if(size-i==1){
				return i;
			}
			else if( value>=sum2&&value<sum2+probabs[i+1]){
				return i+1;
			}
			sum2+=probabs[i+1];
		}
		return 0;


	}



}
//Outer classes section
[System.Serializable]
public class World{
     
	public List<Tracks> tracks=new List<Tracks>();
				public int minTracklength;
				public int maxTracklength;
   public float cacheProbability;
	public float probability=1;
    public float minAppearLimit = 0f;
    public float maxAppearLimit = -1f;
	public List<bool> showTrack=new List<bool>();
    public int list1 = 0;
    public int list2 = 0;


}
[System.Serializable]
public class Tracks{
	public GameObject track;
	public List<GameObject> pools;
	public float probability=1;
}