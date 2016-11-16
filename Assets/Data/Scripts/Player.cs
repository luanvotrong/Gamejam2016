using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private float MOVING_SPEED = 5;
	private float SHOOTING_FORCE_BASE = 20;
	public enum MOVING_STATE {
		STILL = 0,
		LEFT,
		RIGHT
	}
	private MOVING_STATE m_movingState;

	public enum SHOOTING_STATE
	{
		READY = 0,
		AIM,
		FIRE
	}
	private SHOOTING_STATE m_shootingState;
	private Vector2 m_projectileDir;

	private Transform m_transform;
	private Transform m_projectilePivot;

	public GameObject m_projectTilePrefab;

	// Use this for initialization
	void Start () {
		m_transform = GetComponent<Transform> ();
		m_projectilePivot = m_transform.Find ("ProjectilePivot");
		SetMovingState (MOVING_STATE.STILL);
		SetShootingState (SHOOTING_STATE.READY);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		Vector3 pos = m_transform.position;

		switch (m_movingState) {
		case MOVING_STATE.STILL:
			break;
		case MOVING_STATE.LEFT:
			pos.x -= MOVING_SPEED * Time.deltaTime;
			break;
		case MOVING_STATE.RIGHT:
			pos.x += MOVING_SPEED * Time.deltaTime;
			break;
		}

		m_transform.position = pos;
	}

	public void SetMovingState(MOVING_STATE state) {
		m_movingState = state;
	}

	public MOVING_STATE GetMovingState() {
		return m_movingState;
	}

	public void SetShootingState(SHOOTING_STATE state) {
		m_shootingState = state;
		switch (m_shootingState) {
		case SHOOTING_STATE.READY:
			break;
		case SHOOTING_STATE.AIM:
			Debug.Log("AIM: " + m_projectileDir.x + " " + m_projectileDir.y);
			break;
		case SHOOTING_STATE.FIRE:
			Debug.Log ("FIRE: " + m_projectileDir.x + " " + m_projectileDir.y);

			GameObject projectile = (GameObject)Instantiate (m_projectTilePrefab, m_projectilePivot);
			projectile.GetComponent<Rigidbody2D> ().AddForce (m_projectileDir * SHOOTING_FORCE_BASE);
			SetShootingState (SHOOTING_STATE.READY);
			break;
		}
	}

	public SHOOTING_STATE GetShootingState() {
		return m_shootingState;
	}

	public void OnAiming(Vector3 dir) {
		m_projectileDir = dir;
		SetShootingState (SHOOTING_STATE.AIM);
	}

	public void OnFiring(Vector3 dir) {
		m_projectileDir = dir;
		SetShootingState (SHOOTING_STATE.FIRE);
	}
}
