using System;
using System.Threading;
using Assets;
using Assets.Scripts;
using MathUtil;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PixelDestruction : MonoBehaviour
{
	public Texture2D SourceTexture; // original image
	public Material RenderMat; // our render material
	public Material dPixelRenderMat;

    public TerrainMesh NewTerrainMesh;
    public World World;
	public int DestructionResolution = 1; // the resolution of our destruction in pixels

	public Explode explode; // when a player shoots, we blow up the terrain
	private Controls controls; // our players control script
	public Player player; // our player script
    public CameraFollow targetCamera;

	private List<Vector2> previousPlayerPixels = new List<Vector2> ();
	private List<Vector2> previousDynamicPixels = new List<Vector2> ();

    //List<PixelParticle> SpawnedPixels = new List<PixelParticle>(); // a list of all active dPixels
	
	private Color32[] clearPixels;

	// to allow use of coroutines in non-monobehavior scripts
	public void StartShootTimer()
	{
		StartCoroutine("ShootTimer");
	}

	IEnumerator ShootTimer()
	{
		if(!player.canShoot)
		{
			yield return new WaitForSeconds(0.15f);
			player.canShoot = true;
		}
	}

    private void Awake()
    {
        Application.targetFrameRate = -1;
        //Camera.main.orthographicSize = ((long)Screen.height/2f);

        Game.Physics = new CustomPhysics(); // initialize the physics
        explode = new Explode();
        explode.pD = this;
        player = new Player(new Vector2(-100, 500)); // create the player
        player.pD = this;
        Game.Physics.Add(player); // Add player to physics
        targetCamera = Camera.main.GetComponent<CameraFollow>();
    }

    void Start()
    {
	    this.NewTerrainMesh.InitializeWithTerrainTexture(SourceTexture);
	    this.World = this.NewTerrainMesh.Terrain;
	    Game.World = this.World;
	}
	
	// UpdateSimulation is called once per frame
	void FixedUpdate()
	{
	    Game.SimulationStep = Time.fixedDeltaTime;
        //	Debug.Log("active physics objects: " + physics.activePhysicsObjects);

		Profiler.BeginSample("Game.UpdateSimulation");

		//controls.FixedUpdate ();
		//CalculateWorld ();

	    Game.Physics.Update();

        Profiler.EndSample();
	}

    void Update()
    {
        Game.Physics.UpdateVisual(Time.deltaTime);
        //targetCamera.TargetPosition = player.Pos;
    }

}