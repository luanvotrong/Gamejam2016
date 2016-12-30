using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float m_bombThrowingForceBase;

	private Rigidbody2D m_rigidBody;
	private Collider2D m_collider;
	private PlayerBehavior m_playerBehavior;
	private BombController m_bombController;

	public enum CONTROLLER_STATE {
		NONE = 0,
		MOVING,
		BOMBING
	}
	public CONTROLLER_STATE m_state;

    private void SetState(CONTROLLER_STATE state)
    {
        m_state = state;
    }

    // Use this for initialization
    void Start () {
		m_rigidBody = GetComponent<Rigidbody2D> ();
		m_collider = GetComponent<Collider2D> ();
		m_playerBehavior = GetComponent<PlayerBehavior> ();
		m_bombController = GetComponent<BombController> ();

		SetState (CONTROLLER_STATE.NONE);
	}

    // Update is called once per frame
    void Update()
    {
		switch (m_playerBehavior.GetMovingState ()) {
		case PlayerBehavior.MOVING_STATE.STILL:
		case PlayerBehavior.MOVING_STATE.MOVING:
			switch (m_state) {
			case CONTROLLER_STATE.NONE:
				if (Input.GetMouseButtonDown (0)) {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					Debug.Log ("Update::PControler: " + mousePos);
					Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);

					if (m_collider.OverlapPoint (mousePos2D)) {
						SetState (CONTROLLER_STATE.BOMBING);
						//Setup Bomb movement
						m_playerBehavior.SetBombState (PlayerBehavior.BOMB_STATE.READY);
					} else {
						SetState (CONTROLLER_STATE.MOVING);
						//Setup player moving
						Vector2 dir = mousePos2D - m_rigidBody.position;
						dir.Normalize ();

						m_playerBehavior.SetMoveTo (mousePos);
						m_bombController.SetBombState (BombController.BOMB_STATE.READY);
					}
				}
				break;
			case CONTROLLER_STATE.BOMBING:
				if (Input.GetMouseButton (0)) {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);

					Vector2 dir = m_rigidBody.position - mousePos2D;
					dir *= m_bombThrowingForceBase;

					m_bombController.SetAim (dir);
				} else if (Input.GetMouseButtonUp (0)) {
					SetState (CONTROLLER_STATE.NONE);
					m_playerBehavior.SetBombState (PlayerBehavior.BOMB_STATE.THROWING);
					m_bombController.SetBombState (BombController.BOMB_STATE.THROWING);
				}
				break;
			case CONTROLLER_STATE.MOVING:
				if (Input.GetMouseButton (0)) {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);

					Vector2 dir = mousePos2D - m_rigidBody.position;
					dir.Normalize ();

					m_playerBehavior.SetMoveTo (mousePos);
				} else if (Input.GetMouseButtonUp (0)) {
					SetState (CONTROLLER_STATE.NONE);
					//m_playerBehavior.SetMovingDirection (Vector2.zero);
				}
				break;
			}
			break;
		}
    }
}
