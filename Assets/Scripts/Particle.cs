using System;
using System.Linq;
using Assets;
using Assets.Scripts;
using MathUtil;
using UnityEngine;
using System.Collections;


public class PixelParticle : Particle
{
    public Color Color { get; set; }

    private DynamicLayerEntry worldEntry;

    protected PixelParticle(Color color, Vector2 position, Vector2 velocity) : base(position, velocity, 1)
    {
        this.Color = color;
        this.worldEntry = Game.World.CreateDecal(new Vector3I((int)position.x, (int)position.y, 1), color);
    }

    public static PixelParticle CreateAndAddToPhysics(Color color, Vector2 position, Vector2 velocity)
    {
        return new PixelParticle( color, position, velocity){ Stickiness = 1000 };
    }

    public override void UpdateSimulation()
    {
        base.UpdateSimulation();
    }

    public override void FrameEnd()
    {
        base.FrameEnd();

        if (this.worldEntry != null)
            this.worldEntry.MoveTo(new Vector2I((int)this.Pos.x, (int)this.Pos.y));
    }

    public override void Destroy()
    {
        base.Destroy();

        Game.World.RemoveDecal(this.worldEntry);
        this.worldEntry = null;
    }

    /* Collide */
    // called whenever checkConstraints() detects a solid pixel in the way
    public override void Collide(int thisX, int thisY, int thatX, int thatY)
    {
        // first determine if we should stick or if we should bounce
        if (this.Velocity.x * this.Velocity.x + this.Velocity.y * this.Velocity.y < Stickiness * Stickiness)
        { // if the velocity's length is less than our stickiness property, Add the pixel
            // draw a rectangle by looping from x to size, and from y to size
            for (int i = 0; i < this.Size.x; i++)
            {
                for (int j = 0; j < this.Size.x; j++)
                {
                    if (Game.World.IsInside(new Vector2I(thisX + i, thisY + j)))
                    {
                        Game.World.ChangeTerrainColorAt(new Vector2I(thisX + i, thisY + j), Color);
                    }
                }
            }
            // Remove this dynamic pixel
            Destroy();
        }
        else
        { // otherwise, bounce
            // find the normal at the collision point

            // to do this, we need to reflect the velocity across the edge normal at the collision point
            // this is done using a 2D vector reflection formula ( http://en.wikipedia.org/wiki/Reflection_(mathematics) )

            Vector2 pixelNormal = Game.Terrain.GetNormal((int)thatX, (int)thatY);
            if (pixelNormal != Vector2.zero)
            {
                float d = 2 * (this.Velocity.x * pixelNormal.x + this.Velocity.y * pixelNormal.y);
                this.Velocity = new Vector2(this.Velocity.x - pixelNormal.x * d, this.Velocity.y - pixelNormal.y * d);
            }

            // apply bounce friction
            this.Velocity *= bounceFriction;

            // reset x and y so that the pixel starts at point of collision
            this.Pos = new Vector2(thisX, thisY);
        }
    }
}

/* Dynamic Pixel */
// Pixels in motion
public abstract class Particle : MovableObject
{
    public Vector2I Size { get; protected set; }

    protected Vector2 prevPos { get; set; }

    public float Stickiness = 1500; // minimum speed for this pixel to stick
	protected float bounceFriction = 0.85f; // scalar multiplied to velocity after bouncing
  
    protected Particle(Vector2 position, Vector2 velocity, int size)
	{
	    this.Pos = position;
	    this.Velocity = velocity;
	    
        this.prevPos = position;
		this.Size = new Vector2I(size, size);

        if (!Game.World.IsInside(new Vector2I((int)position.x, (int)position.y)))
        {
            Debug.LogError("ERROR: Attempted to create dynamic pixel out of bounds!");
            return;
        }

        //Game.Physics.Add(this);
	}

    public virtual void Destroy()
    {
        Game.Physics.Remove(this);
    }

    public virtual void Collide(int thisX, int thisY, int thatX, int thatY)
    {
        
    }
	
  
	// ResolveCollision, also implemented as a PhysicsObj
	public override void ResolveCollision ()
	{
        Profiler.BeginSample("Particle.ResolveCollision");
 		// Boundary constraints... only Remove the pixel if it exits the sides or bottom of the map
        if (!Game.World.IsInside(new Vector2I((int)this.Pos.x + 1, (int)this.Pos.y + 1)) || !Game.World.IsInside(new Vector2I((int)this.Pos.x - 1, (int)this.Pos.y - 1)))
		{ // we do + or - 1 to destroy before it gets stuck to sides/top/bottom of terrain edges
			Destroy();
//			renderer.Remove (this);
//			physics.Remove (this);
            Profiler.EndSample();
			return;
		}

		// Find if there's a collision between the current and last points
		int[] collision = CustomRayCast.rayCast((int)this.prevPos.x, (int)this.prevPos.y, (int)this.Pos.x, (int)this.Pos.y);
		if (collision.Length > 0) 
			Collide (collision [0], collision [1], collision [2], collision [3]);
		
		// reset last positions
	    this.prevPos = this.Pos;
        Profiler.EndSample();
	}


}