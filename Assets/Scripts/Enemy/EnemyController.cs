using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float m_bombThrowingForceBase;
	public GameObject m_EnemyGround;
	public GameObject m_player;
	public float m_aimOffsetX;
	public float m_aimOffsetY;

	private Bounds m_groundBorder;
	private Rigidbody2D m_rigidBody;
	private Collider2D m_collider;
	private Transform m_playerTransform;
	private EnemyBehavior m_enemyBehavior;
	private BombController m_bombController;

	//AI params
	public float m_updateReadyInterval;
	public float m_updateMovingInterval;
	public float m_updateAimInterval;
	public float m_updateFireInterval;

	private float m_stateUpdateTimer;

	public enum AI_STATE
	{
		READY = 0,
		MOVING,
		AIM,
		FIRE
	}
	private AI_STATE m_aiState;

	public void SetAIState(AI_STATE state) {
		m_aiState = state;
		switch (m_aiState) {
		case AI_STATE.READY:
			m_stateUpdateTimer = m_updateReadyInterval;
			break;
		case AI_STATE.MOVING:
			m_enemyBehavior.SetMoveTo (GenerateMovingTarget());
			m_stateUpdateTimer = m_updateMovingInterval;
			break;
		case AI_STATE.AIM:
			m_stateUpdateTimer = m_updateAimInterval;
			break;
		case AI_STATE.FIRE:
			m_enemyBehavior.SetBombState (EnemyBehavior.BOMB_STATE.THROWING);
			m_bombController.SetBombState (BombController.BOMB_STATE.THROWING);
			m_stateUpdateTimer = m_updateFireInterval;
			break;
		}
	}

	// Use this for initialization
	void Start () {
		m_groundBorder = m_EnemyGround.GetComponent<BoxCollider2D> ().bounds;
		m_rigidBody = GetComponent<Rigidbody2D> ();
		m_collider = GetComponent<Collider2D> ();
		m_playerTransform = m_player.GetComponent<Transform> ();
		m_enemyBehavior = GetComponent<EnemyBehavior> ();
		m_bombController = GetComponent<BombController> ();
		m_enemyBehavior.Init ();

		m_stateUpdateTimer = 0;
	}

	// Update is called once per frame
	void Update () {
		switch (m_enemyBehavior.GetMovingState ()) {
		case EnemyBehavior.MOVING_STATE.STILL:
		case EnemyBehavior.MOVING_STATE.MOVING:
			switch (m_aiState) {
			case AI_STATE.READY:
				if (m_stateUpdateTimer > 0) {
					m_stateUpdateTimer -= Time.deltaTime;
				} else {
					SetAIState (AI_STATE.MOVING);
				}
				break;
			case AI_STATE.MOVING:
				if (m_stateUpdateTimer > 0) {
					m_stateUpdateTimer -= Time.deltaTime;
				} else {
					SetAIState (AI_STATE.AIM);
				}
				break;
			case AI_STATE.AIM:
				if (m_stateUpdateTimer > 0) {
					m_stateUpdateTimer -= Time.deltaTime;
					Vector2 aimingPos = m_playerTransform.position;
					aimingPos.x += Random.Range (-m_aimOffsetX, m_aimOffsetX);
					aimingPos.y += Random.Range (-m_aimOffsetY, m_aimOffsetY);
					m_bombController.SetAim (aimingPos);
				} else {
					SetAIState (AI_STATE.FIRE);
				}
				break;
			case AI_STATE.FIRE:
				if (m_enemyBehavior.GetBombState () != EnemyBehavior.BOMB_STATE.THROWING) {
					if (m_stateUpdateTimer > 0) {
						m_stateUpdateTimer -= Time.deltaTime;
					} else {
						SetAIState (AI_STATE.READY);
					}
				}
				break;
			}
			break;
		}
	}

	Vector2 GenerateMovingTarget() {
		return new Vector2 (Random.Range(m_groundBorder.min.x, m_groundBorder.max.x),
			Random.Range(m_groundBorder.min.y, m_groundBorder.max.y));
	}
}
