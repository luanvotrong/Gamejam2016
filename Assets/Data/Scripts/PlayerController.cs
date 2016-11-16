using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	Transform m_transform;
	Player m_player;
	Transform m_projectilePivot;

	// Use this for initialization
	void Start () {
		m_transform = GetComponent<Transform> ();
		m_player = GetComponent<Player> ();
		m_projectilePivot = m_transform.Find ("ProjectilePivot").GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 touch = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 pos = (m_transform.position);

			if (touch.y < pos.y) {
				if (touch.x < pos.x) {
					m_player.SetMovingState (Player.MOVING_STATE.LEFT);
				} else {
					m_player.SetMovingState (Player.MOVING_STATE.RIGHT);
				}
			}
		} else if (Input.GetMouseButton (0)) {
			Vector3 touch = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 pos = (m_transform.position);

			if (touch.y > pos.y) {
				m_player.OnAiming (m_projectilePivot.position - touch);
			}
		} else if (Input.GetMouseButtonUp (0)) {
			if (m_player.GetMovingState () != Player.MOVING_STATE.STILL) {
				m_player.SetMovingState (Player.MOVING_STATE.STILL);
			}
			if (m_player.GetShootingState () != Player.SHOOTING_STATE.READY) {
				m_player.SetShootingState (Player.SHOOTING_STATE.FIRE);
			}
		}
	}
}
