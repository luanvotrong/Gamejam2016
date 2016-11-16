using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private float MOVING_SPEED = 5;
	public enum MOVING_STATE {
		STILL = 0,
		LEFT,
		RIGHT
	}
	private MOVING_STATE m_movingState;

	private Transform m_transform;

	// Use this for initialization
	void Start () {
		m_transform = GetComponent<Transform> ();
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
}
