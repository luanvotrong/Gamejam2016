using System.Linq;
using Assets.Scripts;
using MathUtil;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* PhysicsObj */
// Any object that will need motion integrated will implement this
// these methods allows the Physics class to forward the object's position using its velocity


/* Physics */   
// Apply motion to objects
public class CustomPhysics
{
	long previousTime;
	long currentTime;
	readonly int fixedDeltaTime = 16; // this was "final" in Java...
	float fixedDeltaTimeSeconds; //= (float)fixedDeltaTime / 1000.0;    // I ask myself, why not just do the math ahead of time... perhaps in processing this would work...
	//float fixedDeltaTimeSeconds = 0.016f;
	int leftOverDeltaTime = 0;
	
    // We will be doing lots of looping and less occassionally, inserts and removes
    // List should be fine as long we do our removals smart and never erase from middle
    public List<MovableObject> activeObjects = new List<MovableObject>();

    // Batch together all additions and removals so we don't interfere with looping through the list
    public Queue<MovableObject> newObjects = new Queue<MovableObject>();
    public Queue<MovableObject> abandonedObjects = new Queue<MovableObject>();
	
	// Constructor
	public CustomPhysics ()
	{

	}

    public void Add(MovableObject obj)
	{
        //this.newObjects.Enqueue(obj);
        // Switched to immediate insertion
        this.activeObjects.Add(obj);
	}

    public void Remove(MovableObject obj)
	{
        this.abandonedObjects.Enqueue(obj);
	}
  
	// integrate motion
	public void Update ()
	{
		// This game uses fixed-sized timesteps.
		// The amount of time elapsed since last UpdateSimulation is split up into units of 16 ms
		// any left over is pushed over to the next UpdateSimulation
		// we take those units of 16 ms and UpdateSimulation the simulation that many times.
		// a fixed timestep will make collision detection and handling (in the Player class, esp.) a lot simpler
		// A low framerate will not compromise any collision detections, while it'll still run at a consistent speed. 
		
        Profiler.BeginSample("Physics.Update");

        ProcessQueuedRemovals();
	    ProcessQueuedAdditions();

		fixedDeltaTimeSeconds = (float)fixedDeltaTime / 1000.0f; // added this here instead of in initializer
		currentTime = (long)Time.time * 1000; //millis (); // time passed * 1000 = milliseconds passed... casted as long...
		long deltaTimeMS = currentTime - previousTime; // how much time has elapsed since the last UpdateSimulation
    
		previousTime = currentTime; // reset previousTime
    
		// Find out how many timesteps we can fit inside the elapsed time
		int timeStepAmt = (int)((float)(deltaTimeMS + leftOverDeltaTime) / (float)fixedDeltaTime); 
    
		// Limit the timestep amount to prevent freezing
		timeStepAmt = Mathf.Min(timeStepAmt, 1);//min (timeStepAmt, 1);
  
		// store left over time for the next frame
		leftOverDeltaTime = (int)deltaTimeMS - (timeStepAmt * (int)fixedDeltaTime);
    
        Profiler.BeginSample("Physics.Update.Loop");

		for (int iteration = 1; iteration <= timeStepAmt; iteration++)
		{
            for (int i =0; i < activeObjects.Count; ++i)
		    {
                Profiler.BeginSample("Physics.Update.Check");
				CheckPhysics(this.activeObjects[i]); // check its physics
                Profiler.EndSample();
                // Temporary home

                Profiler.BeginSample("Physics.Update.FrameEnd");
                this.activeObjects[i].FrameEnd();
                Profiler.EndSample();
			}
		}
        Profiler.EndSample();

        Profiler.EndSample();
	}

    public void UpdateVisual(float timeElapsed)
    {
        foreach (var activeObject in activeObjects)
        {
            activeObject.UpdateVisual(timeElapsed);
        }
    }

    private void ProcessQueuedAdditions()
    {
        //this.activeObjects.AddRange(this.newObjects);
        //this.newObjects.Clear();
    }

    private void ProcessQueuedRemovals()
    {
        Profiler.BeginSample("Physics.ProcessQueuedRemovals");

        // Optimized for large number of removals: 
        // Iterate through active objects only once and use HashSet for lookup 
        var lookup = new HashSet<MovableObject>(this.abandonedObjects);

        for (int i = 0; i < this.activeObjects.Count; ++i)
        {
            if (lookup.Contains(this.activeObjects[i]))
            {
                // Prevent moving a part of the entire list by doing a swap-erase:
                // Swap with last element, then Remove from end
                this.activeObjects[i--] = this.activeObjects.Last();
                this.activeObjects.RemoveAt(this.activeObjects.Count - 1);
            }
        }

        //foreach (var obj in this.abandonedObjects)
        //{
        //    // Prevent moving a part of the entire list by doing a swap-erase
        //    int index = this.activeObjects.IndexOf(obj);
        //    if (index == -1)
        //        Debug.LogWarning("Could not find physicsObj in list of activeObjects");
        //    else
        //    {
        //        // Swap with last element, then Remove from end
        //        this.activeObjects[index] = this.activeObjects.Last();
        //        this.activeObjects.RemoveAt(this.activeObjects.Count - 1);
        //    }
        //}
        
        this.abandonedObjects.Clear();
        
        Profiler.EndSample();
    }

    void CheckPhysics(MovableObject obj)
	{
		// Add gravity
        obj.Velocity = new Vector2(obj.Velocity.x, obj.Velocity.y - 980*fixedDeltaTimeSeconds);

		// if it's a player, only Add y velocity if he's not on the ground.
        //var player = obj as Player;
        //if(player != null && player.onGround)
        //    player.Velocity = new Vector2(player.Velocity.x, 0);

        Profiler.BeginSample("Physics.UpdateSimulation");
        obj.UpdateSimulation();
        Profiler.EndSample();

		// allow the object to do collision detection and other things
        Profiler.BeginSample("Physics.ResolveCollision");
		obj.ResolveCollision ();
        Profiler.EndSample();
	}


}