using System;
using Assets;
using Assets.Scripts;
using MathUtil;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Collision = Assets.Scripts.Collision;
using Random = UnityEngine.Random;

/* Player object */
// Our little box that runs around the map
public class Player : CollidableObject
{
	public Texture2D PlayerTexture;
	public float ShootDelayTime = 0.15f; // 150 milliseconds was default in the java/processing project, so .15 is default here

	// variables to track whether or not the user is pressing A/D
	bool goLeft;
	bool goRight;

	// Are we shooting?
	bool shooting;
	bool shootingAlt;

	// last time (ms) a bullet was shot, used to limit the firing rate
//	long lastShot;

	// variables for physics
	public bool onGround; // are we allowed to jump?
	bool topBlocked;
	public int playerWidth, playerHeight;

    protected Vector2I intMinBounds;
    protected Vector2I intMaxBounds;

	public bool canShoot = true; // prevents repeated shots in a single click
	

	public PixelDestruction pD;
	public CustomPhysics physics;
	//public CameraFollow targetCamera;
	//CustomRayCast rayCast;
	
	// Constructor
	public Player (Vector2 startPos)
	{
        // initialize the player as a 15x15 px box
        playerWidth = 15;
        playerHeight = 15;
        
        this.Pos = startPos;
        this.Velocity = new Vector2();
	    this.Bounds = new BoundingBox
	                  {
	                      Max = this.Pos + 0.5f*new Vector2(playerWidth, playerHeight),
	                      Min = this.Pos - 0.5f*new Vector2(playerWidth, playerHeight),
	                      Size = new Vector2(playerWidth, playerHeight)
	                  };
	}

	// Shooting toggles
	public void shoot ()
	{
		//Debug.Log("shoot() ran - canShoot: " + canShoot + " - shooting: " + shooting);
		shooting = true;
	}

//	public void stopShooting ()
//	{
//		if (shooting)
//		{
//			shooting = false;
//		}
//	}

	public void shootAlt ()
	{
		if (!shootingAlt)
			shootingAlt = true;
	}

	public void stopShootingAlt ()
	{
		if(shootingAlt)
			shootingAlt = false;  
	}

	// jump
	public void jump ()
	{
		if (onGround && Velocity.y < 300) 
            Velocity.y += 300;
	}

	// moving toggles
	public void moveLeft ()
	{
		goLeft = true;
	}

	public void moveRight ()
	{
		goRight = true;
	}

	public void stopLeft ()
	{
		goLeft = false;
	}

	public void stopRight ()
	{
		goRight = false;
	}

//	IEnumerator ShootTimer()
//	{
//		if(!canShoot)
//		{
//			yield return new WaitForSeconds(ShootDelayTime);
//			canShoot = true;
//		}
//	}

    public override void UpdateSimulation()
    {
        // Shooting code
        if (shooting || shootingAlt)
        {
            // first if were doing primary fire we launch a bullet
            //Debug.Log("shootcode area - shooting: " + shooting + " - canShoot: " + canShoot);
            if (!shootingAlt && canShoot) // if were not ALT firing, and enough time has passed since last shot
            {
                //Debug.Log("Attempting Fire at " + Time.time);
                FirePrimary();
                shooting = false;
                canShoot = false;
                //StartCoroutine("ShootTimer"); // crud this isnt monobehavior...
                pD.StartShootTimer();
            }

            if (!shooting && canShoot)
            {
                FireSecondary();
                shootingAlt = false;
                canShoot = false;
                pD.StartShootTimer();
            }
        }

        // movement
        if (goLeft)
        {
            if (Velocity.x > -100)
                Velocity.x -= 40;
        }
        else if (Velocity.x < 0)
            Velocity.x *= 0.8f; // slow down side-ways velocity if we're not moving left

        if (goRight)
        {
            if (Velocity.x < 100)
                Velocity.x += 40;
        }
        else if (Velocity.x > 0)
            Velocity.x *= 0.8f;

        base.UpdateSimulation();

        this.onGround = false;
    }

    public override void UpdateVisual(float deltaTime)
    {
        base.UpdateVisual(deltaTime);
    }

    protected override void OnCollided(Collision.HitResult hit)
    {
        base.OnCollided(hit);

        // This part relies on GRAVITY
        if (hit.IsHit)
        {
            // We are on ground if we either directly collided with something below or we would collide if our velocity would have been downward
            if (hit.HitVector.y >= -1f || Collision.CollideBoundingBoxWithTerrain(this.Bounds, new Vector2(0, -1f)).IsHit)
            {
                this.onGround = true;
            }

            // Climb non-steep terrain in a primitive way
            // Process left
            if (hit.HitVector.x < 0)
            {
                if (!Collision.CollideBoundingBoxWithTerrain(this.Bounds + new Vector2(-1f, 1f), new Vector2(-1, -1f)).IsHit)
                {
                    // We can scale it
                    this.Pos += new Vector2(-1f, 1f);
                    this.UpdateBoundingBox();
                }
            }
            // Process right
            if (hit.HitVector.x > 0)
            {
                if (!Collision.CollideBoundingBoxWithTerrain(this.Bounds + new Vector2(1f, 1f), new Vector2(1, -1f)).IsHit)
                {
                    // We can scale it
                    this.Pos += new Vector2(1f, 1f);
                    this.UpdateBoundingBox();
                }
            }
        }
    }

    void FirePrimary()
	{
		// Create a vector between the player and the mouse, then normalize that vector (to change its length to 1)
		// after multiplying by the desired bullet speed, we get how fast along each axis we want the bullet to be traveling

		// convert mouse pos from screen coordinates to world coordinates
		Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// compare mouse pos to our position, this will be used to aim our projectile
		float diffX = mouseWorld.x - (this.Pos.x);
        float diffY = mouseWorld.y - (this.Pos.y);

		Vector2 diffNormal = new Vector2(diffX, diffY).normalized;
		diffX = diffNormal.x;
		diffY = diffNormal.y;

		// create the bullet at 1500 px/sec, and Add it to our Physics
        Bullet bullet = new Bullet(this.Pos, new Vector2(1500 * diffX, 1500 * diffY));
		bullet.pD = pD; // set the pixel destruction reference
		bullet.Start(); // init the bullet
	}

	void FireSecondary()
	{
		// Create a vector between the player and the mouse, then normalize that vector (to change its length to 1)
		// after multiplying by the desired bullet speed, we get how fast along each axis we want the bullet to be traveling

		// convert mouse pos from screen coordinates to world coordinates
		Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		// compare mouse pos to our position, this will be used to aim our projectile
		float diffX = mouseWorld.x - (this.Pos.x);
        float diffY = mouseWorld.y - (this.Pos.y);
		
		Vector2 diffNormal = new Vector2(diffX, diffY).normalized;
		diffX = diffNormal.x;
		diffY = diffNormal.y;

		int sprayCount = 100; // originally 150 - lowered to improve performance
		sprayCount = sprayCount / pD.DestructionResolution; // we lower the total fired colored pixels if resolution is higher

		for (int i = 0; i < sprayCount; i++)
		{
			float rand1 = Random.Range(0, 255);
			float rand2 = Random.Range(0, 255);
			float rand3 = Random.Range(0, 255);
			Vector3 randoms = new Vector3(rand1, rand2, rand3).normalized;
			rand1 = randoms.x;
			rand2 = randoms.y;
			rand3 = randoms.z; // a hackey way to get normalized (0-1 range) floats

			// create dynamic pixels
			PixelParticle.CreateAndAddToPhysics(new Color (rand1, rand2, rand3, 1), // color
                                    new Vector2(this.Pos.x, this.Pos.y), // position
									new Vector2(Random.Range (-50, 50) + Random.Range (350, 500) * diffX, Random.Range (-50, 50) + Random.Range (350, 500) * diffY) // speed
		              				); 
		}
	}

}