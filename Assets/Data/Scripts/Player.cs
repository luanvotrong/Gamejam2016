using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	private float MOVING_SPEED = 5;
	private float SHOOTING_FORCE_BASE = 500;
	private int DOT_COUNT = 20;
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
	public GameObject m_dotPrefab;

	private Rigidbody2D m_projectileRigid;
	private List<GameObject> m_dots = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		m_transform = GetComponent<Transform> ();
		m_projectilePivot = m_transform.Find ("ProjectilePivot");
		m_projectileRigid = m_projectTilePrefab.GetComponent<Rigidbody2D> ();
		SetMovingState (MOVING_STATE.STILL);
		SetShootingState (SHOOTING_STATE.READY);

		if (m_dots.Count < 1) {
			for (int i = 0; i < DOT_COUNT; i++) {
				GameObject dot = (GameObject)Instantiate (m_dotPrefab);
				m_dots.Add (dot);
			}
		}
		ResetTrajectory ();
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
			Debug.Log ("AIM: " + m_projectileDir.x + " " + m_projectileDir.y);

			SetTrajectory ();
			break;
		case SHOOTING_STATE.FIRE:
			Debug.Log ("FIRE: " + m_projectileDir.x + " " + m_projectileDir.y);

			GameObject projectile = (GameObject)Instantiate (m_projectTilePrefab);
			projectile.GetComponent<Transform> ().position = m_projectilePivot.position;
			projectile.GetComponent<Rigidbody2D> ().AddForce (GetShootingForce());
			SetShootingState (SHOOTING_STATE.READY);

			ResetTrajectory ();
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

	private Vector2 GetShootingForce() {
		return m_projectileDir * SHOOTING_FORCE_BASE;
	}

	private void ResetTrajectory() {
		float offset = (float)(0.2 / 20);
		for (int i = 0; i < m_dots.Count; i++) {
			//m_dots[i].GetComponent<Transform> ().localScale -= new Vector3 (offset * i, offset * i, offset * i);
		}
	}

	private void SetTrajectory() {
		Vector2 force = GetShootingForce () / m_projectileRigid.mass;
		Vector3 startPos = m_projectilePivot.GetComponent<Transform> ().position;
		float velocity = Mathf.Sqrt (force.x * force.x + force.y * force.y);
		float angle = Mathf.Rad2Deg * Mathf.Atan2(force.y, force.y);
		float drag = m_projectileRigid.drag;

		float dt = 0.1f;
		for (int i = 0; i < m_dots.Count; i++) {
			float dx = velocity * dt * Mathf.Cos(angle * Mathf.Deg2Rad);
			float dy = velocity * dt * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * dt * dt / 2.0f);
			Vector3 pos = new Vector3(startPos.x + dx , startPos.y + dy ,2);
			m_dots [i].GetComponent<Transform> ().position = pos;
			velocity *= 1 / (1 + drag * dt);
			dt += 0.1f;
		}
	}
}
