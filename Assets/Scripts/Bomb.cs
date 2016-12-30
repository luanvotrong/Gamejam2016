using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Bomb : MonoBehaviour {

	public float m_speed;
	public int m_radius;

	private Transform m_transform;

	private Vector2 m_dir;
	private float m_distance;
	private float m_maxDistance;

	private SkeletonAnimation m_skeletonAnimation;
    public bool isBombAtt;
	public enum STATE
	{
		NONE = 0,
		MOVING,
		EXPLODE,
		EXPLODED
	}
	private STATE m_state;

	public bool IsActive() {
		return m_state != STATE.NONE; 
	}

	public void SetState (STATE state)	{
		m_state = state;
		switch (m_state) {
		case STATE.NONE:
			this.gameObject.SetActive (false);
			break;
		case STATE.MOVING:
			this.gameObject.SetActive (true);
			break;
		case STATE.EXPLODE:
			//TODO: anim
			SetState(STATE.EXPLODED);
			break;
		case STATE.EXPLODED:
			GameControl.intance.curPixelMap.GetComponent<PixelDestruct> ().Explode ((int)m_transform.position.x, 
				(int)m_transform.position.y,
				m_radius);

                PlayEffExp(transform.position);
			break;
		}
	}

	public STATE GetState() {
		return m_state;
	}

	public void SetThrow(Vector2 currentPos, Vector2 targetPos) {
		m_transform.position = currentPos;

		Vector2 offset = targetPos - currentPos;
		m_maxDistance = offset.magnitude;
		m_distance = m_maxDistance;
		m_dir = offset.normalized;
		m_skeletonAnimation.loop = true;
		m_skeletonAnimation.AnimationName = "indle";
		SetState (STATE.MOVING);
	}

	void OnAnimationCompleted (Spine.TrackEntry trackentry) {
		if (trackentry.Animation.Name == "cowndown") {
			if (trackentry.IsComplete) {
				SetState (STATE.EXPLODE);
			}
		}
	}

	// Use this for initialization
	public void Init (int radius) {
		m_transform = GetComponent<Transform> ();
		m_skeletonAnimation = GetComponent<SkeletonAnimation> ();
		m_skeletonAnimation.AnimationState.Complete += OnAnimationCompleted;
		m_radius = radius;
		SetState (STATE.NONE);
        isBombAtt = false;
    }
	
	// Update is called once per frame
	void Update () {
		UpdateCountDown (Time.deltaTime);
	}

	void UpdateCountDown(float dt) {
		switch (m_state) {
		case STATE.NONE:
			break;
		case STATE.MOVING:
			break;
		case STATE.EXPLODE:
			break;
		}
	}

	void FixedUpdate() {
		FixedUpdateMoving (Time.fixedDeltaTime);
	}

	void FixedUpdateMoving (float dt) {
		switch (m_state) {
		case STATE.NONE:
			break;
		case STATE.MOVING:
                if (m_distance > 0)
                {
                    float moveOffset = m_speed * Time.fixedDeltaTime;
                    m_distance -= moveOffset;
                    Vector2 vectorOffset = m_dir * moveOffset;
                    m_transform.position += new Vector3(vectorOffset.x, vectorOffset.y);

                    //TODO: temporarily scale bomb, have to polish
                    float scale = (1 - Mathf.Abs((m_distance / (m_maxDistance / 2)) - 1)) * 10f + 50f;
                    m_transform.localScale = new Vector3(scale, scale, 1);
                }
                else
                {
                    if (!isBombAtt)
                    {
                        isBombAtt = true;
                        PlayEffAdd(transform.localPosition);
					m_skeletonAnimation.loop = false;
					m_skeletonAnimation.AnimationName = "cowndown";
                    }
                }
			break;
		case STATE.EXPLODE:
			break;
		}
	}

	public bool OnBombCollision(Collider2D coll) {
		if (coll.gameObject.tag == "ground") {
			return true;
		}
		return false;
	}

	public Ground.GroundCollider GetCollider() {
		return new Ground.GroundCollider ((Vector2)m_transform.position, m_radius);
	}

    public void PlayEffAdd(Vector3 pos)
    {
        pos.z -= 2;
        GameObject effInst = GameControl.intance.effPool_Bomb1.popFromPool(0);
        effInst.transform.localPosition = pos;
        effInst.SetActive(true);
        effInst.GetComponent<ParticleControl>().ActiveObj(true);
        GameControl.intance.effPool_Bomb1.pushToPools(effInst, 0, GameControl.intance.poolsize_Eff_Bomb1);
        //GameObject effinst = Instantiate(GameControl.intance.eff_Bomb_1, pos, Quaternion.identity) as GameObject;
    }

    public void PlayEffExp(Vector3 pos)
    {
        pos.z -= 2;
        GameObject effInst = GameControl.intance.effPool_Bomb2.popFromPool(0);
        effInst.transform.localPosition = pos;
        effInst.SetActive(true);
        effInst.GetComponent<ParticleControl>().ActiveObj(true);
        GameControl.intance.effPool_Bomb2.pushToPools(effInst, 0, GameControl.intance.poolsize_Eff_Bomb2);
        //GameObject effinst = Instantiate(GameControl.intance.eff_Bomb_2, pos, Quaternion.identity) as GameObject;
    }
}
