using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {
    private static GameLoop _instance;

    private ResourceMng resourceMng;
    public bool isDataLoaded;

    private AudioClip[] mySounds;

    public static GameLoop instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<GameLoop>();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	void Awake() 
	{
        
		if(_instance == null)
		{
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != _instance)
				Destroy(this.gameObject);
		}
        isDataLoaded = false;
	}

    void Start()
    {
        LoadAllData();
    }

    public void LoadAllData()
    {
        if (isDataLoaded)
            return;

        // Loading resources
        LoadResource();

        isDataLoaded = true;
    }

    private void LoadResource()
    {
        if (!resourceMng) resourceMng = new ResourceMng();
        //Sounds
        resourceMng.LoadSounds();
        mySounds = resourceMng.getMySound();

        Debug.Log("done LoadResource");
    }

    public AudioClip[] getMySound()
    {
        return mySounds;
    }
	
	void LateUpdate()
	{
		//TODO
	}
}
