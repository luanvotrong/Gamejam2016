using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : MonoBehaviour {

	public struct GroundCollider{
		public float x;
		public float y;
		public float r;

		public GroundCollider (float _x, float _y, float _r){
			x = _x;
			y = _y;
			r = _r;
		}

		public GroundCollider (Vector2 pos, float _r){
			x = pos.x;
			y = pos.y;
			r = _r;
		}

		public Vector2 GetCenter() {
			return new Vector2 (x, y);
		}
	}

	private List<GroundCollider> m_colliders = new List<GroundCollider> ();
	public void AddCollider(float _x, float _y, float _r) {
		m_colliders.Add (new GroundCollider(_x, _y, _r));
	}

	public void AddCollider (GroundCollider collider) {
		m_colliders.Add (collider);
	}

	public List<GroundCollider> GetCollidesCollider(Vector2 pos) {
		List<GroundCollider> res = new List<GroundCollider> ();
		foreach(GroundCollider collider in m_colliders) {
			var dx = pos.x - collider.x;
			var dy = pos.y - collider.y;
			if (Mathf.Abs (dx) > collider.r || Mathf.Abs(dy) > collider.r) {
				continue;
			}

			if (Mathf.Sqrt (dx * dx + dy * dy) < collider.r) {
				res.Add (collider);
			}
		}

		return res;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
