using Assets.Scripts;
using UnityEngine;
using System.Collections;

/* Bullet */
// Acts similarly to PhysicsPixel
class Bullet : CollidableObject
{
	public int ID = -1; // -1 is an invalid ID, and needs to be changed after spawn

    Vector2 lastPos; // last position
	
	//DestructibleTerrain dT; // these vars and Start() are all about gettin everything connected...
	public PixelDestruction pD;
	//CustomTerrain terrain;
	//CustomRenderer renderer;
	Explode explode;
	
	//bool firstRun = true;
	
	public void Start()
	{
		explode = pD.explode;
	}
	
	// Constructor
	public Bullet (Vector2 position, Vector2 velocity)
	{
	    this.Pos = position;
	    this.Velocity = velocity;
	    this.lastPos = this.Pos;
        Game.Physics.Add(this);
	}
  
	public override void ResolveCollision ()
	{
        int[] collision = CustomRayCast.rayCast((int)this.lastPos.x, (int)this.lastPos.y, (int)this.Pos.x, (int)this.Pos.y);
		if (collision.Length > 0)
		{
//			pD.DestroyBullet();
			Game.Physics.Remove (this);
			explode.explode (collision [2], collision [3], 70); //60);
		}
	    this.lastPos = this.Pos;
	}
}