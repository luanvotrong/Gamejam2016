using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class IGMMng : MonoBehaviour {
    public static IGMMng instance;
    public enum STATE
    {
        NONE = 0,
        IGM,
        PAUSEGAME,
        WIN,
        LOSE,
    }

    public STATE curState;
    public STATE nextState;

    public GameObject btnPause;
    public GameObject pausePanel;
    public GameObject LosePanel;
    public GameObject winPanel;

    public Text PlayerHead;
    public Text BossHead;

    public Text iSlandLV;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        curState = STATE.NONE;
        nextState = STATE.IGM;
    }

    void Update()
    {
        if (curState != nextState)
        {
            OnChangeState();
        }

        switch (curState)
        {
            case STATE.IGM:
                break;
        }
    }

    void OnChangeState()
    {
        switch (nextState)
        {
            case STATE.IGM:
                Time.timeScale = 1;
                pausePanel.SetActive(false);
                LosePanel.SetActive(false);
                winPanel.SetActive(false);
                break;
            case STATE.PAUSEGAME:
                Time.timeScale = 0;
                pausePanel.SetActive(true);
                break;
            case STATE.WIN:
                //Time.timeScale = 0;
                winPanel.SetActive(true);
                StartCoroutine(WaitAndStartNewGame(2));
                break;
            case STATE.LOSE:
                Time.timeScale = 0;
                LosePanel.SetActive(true);
                break;
        }
        curState = nextState;
    }

    public void PasueClick()
    {
        nextState = STATE.PAUSEGAME;
    }

    public void ResetClick()
    {
        nextState = STATE.IGM;
        GameControl.intance.Restart();
    }

    public void ResumeClick()
    {
        nextState = STATE.IGM;
    }

    public void MainMenuClick()
    {
        nextState = STATE.IGM;
        SceneManager.LoadScene(0);
    }

    IEnumerator WaitAndStartNewGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nextState = STATE.IGM;
        //winPanel.SetActive(false);
        print("WaitAndPrint " + Time.time);
    }
}
