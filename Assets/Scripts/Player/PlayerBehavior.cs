using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class PlayerBehavior : MonoBehaviour {
	public float m_targetRadius;
	public int m_userSpeed;
	public GameObject m_PlayerGround;
	private Bounds m_groundBorder;
	private Collider2D m_collider;
	private Transform m_transform;
	private Ground m_ground;

	private Vector2 m_targetPos;
	private Rigidbody2D m_rigidBody;
	private SkeletonAnimation m_skeletonAnimation;
	private BombController m_bombController;

	public int HP = 5;
	public void OnHit() {
		HP--;
		if (HP < 1) {
			SetMovingState (MOVING_STATE.DYING);
		} else {
			m_hitEffectTimer = m_hitEffectDuration;
		}

        IGMMng.instance.PlayerHead.text = HP.ToString();
	}
	private float m_hitEffectDuration = 0.5f;
	private float m_hitEffectTimer = 0f;

	Vector2 m_movingDir = new Vector2 ();

	public Ground.GroundCollider GetFoot() {
		Bounds bound = m_collider.bounds;
		return new Ground.GroundCollider (new Vector2(bound.min.x + (bound.max.x - bound.min.x)/2, bound.min.y), 70);
	}

	public enum MOVING_STATE {
		STILL = 0,
		MOVING,
		DYING,
		DIED
	}
	public MOVING_STATE m_movingState = MOVING_STATE.STILL;
	public MOVING_STATE GetMovingState() {
		return m_movingState;
	}

	public void SetMovingState(MOVING_STATE state) {
		switch (state) {
		case MOVING_STATE.STILL:
			if (m_movingState != state) {
                Debug.Log("PlayerBehavior: MOVING_STATE.STILL");
                    m_skeletonAnimation.loop = true;
                    m_skeletonAnimation.AnimationName = "Indle";
			}
			break;
		case MOVING_STATE.MOVING:
			if (m_movingState != state) {
                Debug.Log("PlayerBehavior: MOVING_STATE.MOVING");
                    m_skeletonAnimation.loop = true;
                    m_skeletonAnimation.AnimationName = "run";
			}
			break;
		case MOVING_STATE.DYING:
			if (m_movingState != state) {
				Debug.Log("PlayerBehavior: MOVING_STATE.DYING");
				m_skeletonAnimation.loop = false;
				m_skeletonAnimation.AnimationName = "die";
			}
			break;
		case MOVING_STATE.DIED:
			if (m_movingState != state) {
				Debug.Log("PlayerBehavior: MOVING_STATE.DIED");
			}
			break;
		}
		m_movingState = state;
	}

	public void SetMoveTo(Vector2 pos) {
		m_targetPos = pos;
		Vector2 dir = (pos - m_rigidBody.position);
		dir.Normalize ();
		SetMovingDirection (dir);
	}

	public void SetMovingDirection(Vector2 dir) {
		//dir = 0, 0 then player stop moving, set state to still
		m_movingDir = dir;
		if (m_movingDir == Vector2.zero) {
			SetMovingState (MOVING_STATE.STILL);
		} else {
			SetMovingState (MOVING_STATE.MOVING);
		}
	}

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
		case BOMB_STATE.THROWING:
			if (m_bombState != state) {
				m_skeletonAnimation.loop = false;
				m_skeletonAnimation.AnimationName = "throw";
			}
			break;
		case BOMB_STATE.FIRE:
			SetBombState (BOMB_STATE.READY);
			break;
		case BOMB_STATE.READY:
			m_skeletonAnimation.loop = true;
			m_skeletonAnimation.AnimationName = "Indle";
			break;
		}
		m_bombState = state;
	}

	void OnAnimationCompleted (Spine.TrackEntry trackentry) {
		if (trackentry.Animation.Name == "throw") {
			if (trackentry.IsComplete) {
				SetBombState (BOMB_STATE.FIRE);
				m_bombController.SetBombState (BombController.BOMB_STATE.FIRE);
			}
		} else if (trackentry.Animation.Name == "die") {
			if (trackentry.IsComplete) {
				SetMovingState (MOVING_STATE.DIED);
			}
		}
	}

	void OnAnimationEnded() {
	}

	public void ResetGameplay() {
		SetMovingState(MOVING_STATE.STILL);
		SetBombState(BOMB_STATE.READY);
		m_transform.position = new Vector3 (0, -500, -20);
		m_bombController.Reset ();
		HP = 5;
	}

	// Use this for initialization
	void Start () {
		m_groundBorder = m_PlayerGround.GetComponent<BoxCollider2D> ().bounds;
		m_collider = GetComponent<BoxCollider2D> ();
		m_transform = GetComponent<Transform> ();
		m_ground = m_PlayerGround.GetComponent<Ground> ();

		m_movingState = MOVING_STATE.STILL;
		m_bombState = BOMB_STATE.READY;

		m_rigidBody = GetComponent<Rigidbody2D> ();
		m_skeletonAnimation = GetComponent<SkeletonAnimation> ();
		m_skeletonAnimation.state.Complete += OnAnimationCompleted;

		m_bombController = GetComponent<BombController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_hitEffectTimer > 0) {
			m_hitEffectTimer -= Time.deltaTime;

			float percent = m_hitEffectTimer / (m_hitEffectDuration / 2) - 1;
			m_transform.localScale = new Vector3(50 - percent*20, 50 + percent*20, 1);
			if (m_hitEffectTimer <= 0) {
				m_transform.localScale = new Vector3 (50, 50, 1);
			}
		}
        IGMMng.instance.PlayerHead.text = HP.ToString();
    }

	void FixedUpdate() {
		UpdateMove (Time.fixedDeltaTime);
	}

	void UpdateMove(float dt) {
		switch (m_movingState) {
		case MOVING_STATE.STILL:
			break;
		case MOVING_STATE.MOVING:
			if (((Vector2)m_transform.position - m_targetPos).magnitude > m_targetRadius) {
				Vector2 pos = m_transform.position;
				pos = pos + (m_movingDir * m_userSpeed * dt);

				List<Ground.GroundCollider> colliders = m_ground.GetCollidesCollider (pos);
				foreach (Ground.GroundCollider collider in colliders) {
					Vector2 ray = new Vector2 (pos.x - collider.x, pos.y - collider.y);
					float offset = ray.magnitude - collider.r;
					ray = ray.normalized * offset;
					pos -= ray;
				}
				pos.x = Mathf.Clamp (pos.x, m_groundBorder.min.x, m_groundBorder.max.x);
				pos.y = Mathf.Clamp (pos.y, m_groundBorder.min.y, m_groundBorder.max.y);

				m_transform.position = pos;
			} else {
				SetMovingState (MOVING_STATE.STILL);
			}
			break;
		}
	}
}
