using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BossLevel : MonoBehaviour  {
	public int hp;
	public int speed;
	public float offsetX;
	public float offsetY;
	public float readyInterval;
	public float movingInterval;
	public float aimInterval;
	public float fireInterval;
}

public class GameControl : MonoBehaviour {
    public static GameControl intance;

    public CameraFollow cameraFollow;

    public GameObject pLayer;
    public GameObject enemy;
	public PopupMgr popupMgr;

	public PlayerBehavior playerBehavior;
	public EnemyBehavior enemyBehavior;

    public GameObject eff_pool;
    public GameObject eff_Bomb_1;
    public int poolsize_Eff_Bomb1 = 1;
    public GameObject eff_Bomb_2;
    public int poolsize_Eff_Bomb2 = 1;
    public PoolManager effPool_Bomb1;
    public PoolManager effPool_Bomb2;

    public int poolsiz_Bomb = 10;
    public GameObject bombPefab;
    public GameObject obj_pool;
    public PoolManager bombPool;

    public GameObject pixelMap;
    public GameObject curPixelMap;

    public int curMapID = 0;
    public bool isChangeMap = false;

    //Sound
    public bool isPlaySoundBG = false;

	public List<Bomb> m_bombList = new List<Bomb> ();
	public List<BossLevel> m_bossLevels = new List<BossLevel> ();
	//Level
	public int m_level = 0;

    void Awake()
    {
        intance = this;
    }

    void Start()
    {
		playerBehavior = pLayer.GetComponent<PlayerBehavior> ();
		enemyBehavior = enemy.GetComponent<EnemyBehavior>();
     
		SetUpBossLevels ();
        isChangeMap = false;
        isPlaySoundBG = false;
        CreateMap(new Vector3(0, 0, 25));
		OnWin ();
        InitEffPool();
        InitBombPool();
    }

	void SetUpBossLevels() {
		m_bossLevels.Clear ();
		BossLevel level;
		//0
		level = new BossLevel ();
		level.hp = 1;
		level.speed = 200;
		level.offsetX = 100f;
		level.offsetY = 100f;
		level.readyInterval = 1f;
		level.movingInterval = 1f;
		level.aimInterval = 1.5f;
		level.fireInterval = 2.5f;
		m_bossLevels.Add (level);

		//2
		level = new BossLevel ();
		level.hp = 2;
		level.speed = 350;
		level.offsetX = 75;
		level.offsetY = 75f;
		level.readyInterval = 1f;
		level.movingInterval = 1f;
		level.aimInterval = 1.5f;
		level.fireInterval = 2.5f;
		m_bossLevels.Add (level);

		//3
		level = new BossLevel ();
		level.hp = 5;
		level.speed = 400;
		level.offsetX = 50;
		level.offsetY = 50;
		level.readyInterval = 0.7f;
		level.movingInterval = 0.8f;
		level.aimInterval = 0.7f;
		level.fireInterval = 2f;
		m_bossLevels.Add (level);
	}

    public void InitEffPool()
    {
        effPool_Bomb1 = new PoolManager(1);
        effPool_Bomb2 = new PoolManager(1);
        int poolsize = poolsize_Eff_Bomb1;
        for (int i = 0; i < poolsize; i++)
        {
            GameObject effinst = Instantiate(eff_Bomb_1, Vector3.zero, Quaternion.identity) as GameObject;
            effinst.transform.parent = eff_pool.transform;
            effinst.transform.name = eff_Bomb_1.name + "-" + i;
            //effinst.transform.localScale = new Vector3(20f, 20f, 20f);
            effinst.SetActive(false);
            effPool_Bomb1.pushToPools(effinst, 0, poolsize);
        }

        poolsize = poolsize_Eff_Bomb2;

        for (int i = 0; i < poolsize; i++)
        {
            GameObject effinst = Instantiate(eff_Bomb_2, Vector3.zero, Quaternion.identity) as GameObject;
            effinst.transform.parent = eff_pool.transform;
            effinst.transform.name = eff_Bomb_2.name + "-" + i;
            //effinst.transform.localScale = new Vector3(20f, 20f, 20f);
            effinst.SetActive(false);
            effPool_Bomb2.pushToPools(effinst, 0, poolsize);
        }
    }

    public void InitBombPool()
    {
        bombPool = new PoolManager(1);
        int poolsize = poolsiz_Bomb;

        for (int i = 0; i < poolsize; i++)
        {
            GameObject bombinst = Instantiate(bombPefab, Vector3.zero, Quaternion.identity) as GameObject;
            bombinst.transform.parent = obj_pool.transform;
            bombinst.transform.name = bombPefab.name + "-" + i;
            //effinst.transform.localScale = new Vector3(20f, 20f, 20f);
            bombinst.SetActive(false);
            bombPool.pushToPools(bombinst, 0, poolsize);
			m_bombList.Add (bombinst.GetComponent<Bomb> ());
        }
    }

    public void CreateMap(Vector3 post)
    {
        if (curPixelMap != null)
        {
            Destroy(curPixelMap);
        }
        GameObject inst = Instantiate(pixelMap, post, Quaternion.identity) as GameObject;
        inst.transform.parent = gameObject.transform;
        //inst.GetComponent<PixelDestruct>().NewTerrainMesh.transform.localPosition = post;

        curPixelMap = inst;
    }

	public void SetupGameplay(int curMapId) {
		pLayer.GetComponent<PlayerBehavior> ().ResetGameplay ();
		enemy.GetComponent<EnemyBehavior> ().ResetGameplay ();
	}

    public void Restart()
    {
        m_level = 0;
        CreateMap(new Vector3(0, 0, 25));
        OnWin();
    }

	public void OnLose() {
		cameraFollow.transform.localPosition = new Vector3(cameraFollow.transform.localPosition.x, 2048 * curMapID, cameraFollow.transform.localPosition.z);
		CreateMap(new Vector3(0, 2048 * curMapID, 25));
		SetupGameplay (curMapID);

		//Reset boss
		m_level--;
		UpdateBossLevels();
		m_level++;

        IGMMng.instance.nextState = IGMMng.STATE.LOSE;
	}

	public void OnWin() {
		cameraFollow.transform.localPosition = new Vector3(cameraFollow.transform.localPosition.x, 2048 * curMapID, cameraFollow.transform.localPosition.z);
		CreateMap(new Vector3(0, 2048 * curMapID, 25));
		SetupGameplay (curMapID);

		//Update boss
		UpdateBossLevels();
        if (m_level > 0)
        {
            IGMMng.instance.nextState = IGMMng.STATE.WIN;
            IGMMng.instance.iSlandLV.text = "Island " + (m_level + 1);
        }
        m_level++;
    }

	public void UpdateBossLevels() {
		BossLevel level = m_bossLevels [m_level < m_bossLevels.Count ? m_level : (m_bossLevels.Count - 1)];
		enemyBehavior.m_userSpeed = level.speed;
		enemyBehavior.HP = level.hp;
		EnemyController crl = enemy.GetComponent<EnemyController> ();
		crl.m_aimOffsetX = level.offsetX;
		crl.m_aimOffsetY = level.offsetY;
		crl.m_updateReadyInterval = level.readyInterval;
		crl.m_updateMovingInterval = level.movingInterval;
		crl.m_updateAimInterval = level.aimInterval;
		crl.m_updateFireInterval = level.fireInterval;
	}

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			/*
			if (isChangeMap)
            {
                cameraFollow.transform.localPosition = new Vector3(cameraFollow.transform.localPosition.x, 2048 * curMapID, cameraFollow.transform.localPosition.z);
                CreateMap(new Vector3(0, 2048 * curMapID, 25));
				SetupGameplay (curMapID);
				curMapID++;
            }
            */
            if (GameLoop.instance && GameLoop.instance.isDataLoaded && !isPlaySoundBG)
            {
                isPlaySoundBG = true;
                SoundManager.instance.PlayMusic(GameLoop.instance.getMySound()[SoundDef.ID_SOUND_INGAME_BG_MUSIC], true, 0.2f);
            }
        }

		if (playerBehavior.GetMovingState () == PlayerBehavior.MOVING_STATE.DIED) {
			OnLose ();
		}
		else if(enemyBehavior.GetMovingState() == EnemyBehavior.MOVING_STATE.DIED) {
			OnWin ();
		}
    }
}
