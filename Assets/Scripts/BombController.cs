using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombController : MonoBehaviour {

	public int MAX_DOT = 20;
	public GameObject m_bombPrefab;
	public int m_bombRadius;
	public bool m_isDrawTrajectory;

	private Vector2 m_target;
	private Transform m_transform;

	public GameObject m_dotPrefab;
	public List<GameObject> m_dots = new List<GameObject> ();
	public List<GameObject> m_grounds = new List<GameObject> ();

	private PlayerBehavior m_playerBehavior;
	private EnemyBehavior m_enemyBehavior;

    public enum BOMB_STATE {
		READY = 0,
		AIM,
		THROWING,
		FIRE
	}
	private BOMB_STATE m_bombState = BOMB_STATE.READY;
	public BOMB_STATE GetBombState() {
		return m_bombState;
	}

	public void SetBombState(BOMB_STATE state) {
		//TODO: switch case set anim and draw trajectory
		switch (state) {
		case BOMB_STATE.READY:
			if (m_isDrawTrajectory) {
				for (int i = 0; i < MAX_DOT; i++) {
					m_dots [i].gameObject.SetActive (false);
				}
			}
			break;
		case BOMB_STATE.AIM:
			break;
		case BOMB_STATE.THROWING:
			break;
		case BOMB_STATE.FIRE:
                //GameObject bomb = GetBombFromPool ();
                GameObject bomb = GameControl.intance.bombPool.popFromPool(0);
                bomb.GetComponent<Bomb>().Init(m_bombRadius);
                bomb.GetComponent<Bomb> ().SetThrow (m_transform.position, m_target);
                GameControl.intance.bombPool.pushToPools(bomb, 0, GameControl.intance.poolsiz_Bomb);
                SetBombState (BOMB_STATE.READY);
			break;
		}
		m_bombState = state;
	}

	public void SetAim(Vector2 target) {
		m_target = target;
		if (m_isDrawTrajectory) {
			Vector2 ray = m_target - (Vector2)m_transform.position;
			Vector2 offset = ray.normalized * ray.magnitude / MAX_DOT;
			for (int i = 0; i < MAX_DOT; i++) {
				m_dots [i].gameObject.SetActive (true);
				m_dots [i].GetComponent<Transform> ().position = m_target - (offset * i);
			}
		}
	}

	public void Reset() {
		for (int i = 0; i < GameControl.intance.m_bombList.Count; i++) {
			Bomb bomb = GameControl.intance.m_bombList [i];
			bomb.SetState (Bomb.STATE.NONE);
		}
	}

	// Use this for initialization
	void Start () {
		m_transform = GetComponent<Transform> ().Find ("BombPos").GetComponent<Transform> ();
		if (m_isDrawTrajectory) {
			for (int i = 0; i < MAX_DOT; i++) {
				GameObject dot = (GameObject)Instantiate (m_dotPrefab);
				dot.gameObject.SetActive (false);
				dot.GetComponent<Transform> ().SetParent (m_transform);
				m_dots.Add (dot);
			}
		}

		m_playerBehavior = GameControl.intance.pLayer.GetComponent<PlayerBehavior> ();
		m_enemyBehavior = GameControl.intance.enemy.GetComponent<EnemyBehavior> ();
		SetBombState (BOMB_STATE.READY);
	}
	
	// Update is called once per frame
	void Update () {
		switch (m_bombState) {
		case BOMB_STATE.READY:
			break;
		case BOMB_STATE.AIM:
			break;
		case BOMB_STATE.FIRE:
			break;
		}

		for (int i = 0; i < GameControl.intance.m_bombList.Count; i++) {
			Bomb bomb = GameControl.intance.m_bombList [i];
			if (bomb.GetState () == Bomb.STATE.EXPLODED) {
				//Handle ground collider
				for(int j=0; j<m_grounds.Count; j++) {
					if (bomb.OnBombCollision (m_grounds [j].GetComponent<Collider2D> ())) {
						m_grounds [j].GetComponent<Ground> ().AddCollider(bomb.GetCollider());
					}
				}

				//Handle player-enemy death
				Ground.GroundCollider collider = bomb.GetCollider();
				Ground.GroundCollider colliderFoot = m_enemyBehavior.GetFoot();
				Vector2 ray = colliderFoot.GetCenter() - collider.GetCenter();
				if (ray.magnitude < (collider.r + colliderFoot.r)) {
					//m_enemyBehavior.SetMovingState (EnemyBehavior.MOVING_STATE.DYING);
					m_enemyBehavior.OnHit();
				}
				colliderFoot = m_playerBehavior.GetFoot();
				ray = colliderFoot.GetCenter() - collider.GetCenter();
				if (ray.magnitude < (collider.r + colliderFoot.r)) {
					//m_playerBehavior.SetMovingState (PlayerBehavior.MOVING_STATE.DYING);
					m_playerBehavior.OnHit();
					if (Random.Range (0, 100) < 50) {
						GameControl.intance.popupMgr.SetType(PopupMgr.POPUP_TYPE.ENEMY_SMILE);
					} else {
						GameControl.intance.popupMgr.SetType(PopupMgr.POPUP_TYPE.PLAYER_DIE);
					}
				}


				//TODO: handle collider;
				bomb.SetState(Bomb.STATE.NONE);
			}
		}
	}
}
