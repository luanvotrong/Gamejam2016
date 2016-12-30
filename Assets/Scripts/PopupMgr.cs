using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupMgr : MonoBehaviour {

	private string[] m_popupNames = { 
		"asset_13",
		"asset_29",
		"asset_74",
		"asset_52",
		"asset_86"
	};

	public enum POPUP_TYPE
	{
		NONE = -1,
		ENEMY_HURT,
		ENEMY_ANGRY,
		PLAYER_SMILE,
		ENEMY_SMILE,
		PLAYER_DIE
	}
	private POPUP_TYPE m_type;
	public void SetType(POPUP_TYPE type){
		switch (m_type) {
		case POPUP_TYPE.NONE:
			break;
		default:
			m_popups [(int)m_type].SetActive (false);
			break;
		}

		m_type = type;
		switch (m_type) {
		case POPUP_TYPE.NONE:
			for (int i = 0; i < m_popups.Count; i++) {
				m_popups [i].SetActive (false);
			}
			break;
		default:
			if (Random.Range (0, 100) < 90) {
				m_popups [(int)m_type].SetActive (true);
			}
			break;
		}

		m_timer = 1.0f;
	}
	private float m_timer;
	private List<GameObject> m_popups = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < m_popupNames.GetLength(0); i++) {
			GameObject popup = (GameObject)GetComponent<Transform> ().Find (m_popupNames [i]).gameObject;
			popup.SetActive (false);
			m_popups.Add (popup);
		}
		m_type = POPUP_TYPE.NONE;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_timer > 0) {
			m_timer -= Time.deltaTime;
			if (m_timer <= 0) {
				SetType (POPUP_TYPE.NONE);
			}
		}
	}
}
