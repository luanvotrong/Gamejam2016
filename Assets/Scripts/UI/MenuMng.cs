using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;

public class MenuMng : MonoBehaviour {
    public static MenuMng instance;

    public bool isFistPlay;

    public enum STATE {
        NONE = 0,
        TUTORIAL,
        MAINMENU,
        INGAME,
    }
    public STATE curState;
    public STATE nextState;

    //Loading Screen
    private const float TIME_LOADING = 3;
    private float timeLoading = 0;
    private int loadingStep = 0;
    public Image loadingScrene;
    public Sprite[] loadingSprite;
    public Sprite backGround;

    //MainMenu
    public Transform mainMenu;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        curState = STATE.NONE;
        nextState = STATE.MAINMENU;
        timeLoading = TIME_LOADING;

        Load();
    }

    void Update()
    {
        if (curState != nextState)
        {
            OnChangeState();
        }

        switch (curState)
        {
            case STATE.TUTORIAL:
                timeLoading -= Time.deltaTime;
                if (timeLoading <= 0)
                {
                    timeLoading = TIME_LOADING;
                    if (loadingStep == 0)
                    {
                        loadingScrene.sprite = loadingSprite[1];
                        loadingStep++;
                    }
                    else if (loadingStep == 1)
                    {
                        nextState = STATE.INGAME;
                    }
                }
                break;
        }
    }

    void OnChangeState()
    {
        switch (nextState)
        {
            case STATE.TUTORIAL:
                loadingScrene.sprite = loadingSprite[0];
                mainMenu.gameObject.SetActive(false);
                break;
            case STATE.MAINMENU:
                loadingScrene.sprite = backGround;
                mainMenu.gameObject.SetActive(true);
                break;
            case STATE.INGAME:
                StartClick();
                break;
        }
        curState = nextState;
    }

    public void StartClick()
    {
        Debug.Log("StartClick");
        if (isFistPlay == false)
        {
            isFistPlay = true;
            nextState = STATE.TUTORIAL;
            Save();
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void Save()
    { 
        PlayerPrefs.SetInt("isFistPlay", (isFistPlay)?1:0);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("isFistPlay"))
        {
            isFistPlay = (PlayerPrefs.GetInt("isFistPlay") == 1)? true : false;
        }
    }

    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }
}
