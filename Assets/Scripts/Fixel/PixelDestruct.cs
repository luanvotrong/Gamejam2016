using System;
using System.Threading;
using Assets;
using Assets.Scripts;
using MathUtil;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PixelDestruct : MonoBehaviour {
    public Texture2D SourceTexture; // original image
    public Material RenderMat; // our render material
    public Material dPixelRenderMat;

    public TerrainMesh NewTerrainMesh;
    public World World;
    public int DestructionResolution = 1; // the resolution of our destruction in pixels
    public int adrNum = 1;

    public PixelExplode explode; // when a player shoots, we blow up the terrain
    public PixelPlayer player; // our player script
    public CameraFollow targetCamera;

    private List<Vector2> previousPlayerPixels = new List<Vector2>();
    private List<Vector2> previousDynamicPixels = new List<Vector2>();

    //List<PixelParticle> SpawnedPixels = new List<PixelParticle>(); // a list of all active dPixels

    private Color32[] clearPixels;

    private void Awake()
    {
        Application.targetFrameRate = -1;
        explode = new PixelExplode();
        explode.pD = this;
        //targetCamera = Camera.main.GetComponent<CameraFollow>();

        //player = new PixelPlayer(new Vector2(-100, 500)); // create the player
        //player.pD = this;
        //Game.Physics.Add(player); // Add player to physics
    }

    void Start()
    {
        this.NewTerrainMesh.InitializeWithTerrainTexture(SourceTexture, adrNum);
        this.World = this.NewTerrainMesh.Terrain;
        Game.World = this.World;
    }

    public void Init()
    {
        Application.targetFrameRate = -1;
        if (explode == null)
        {
            explode = new PixelExplode();
            explode.pD = this;
        }
        
        this.NewTerrainMesh.InitializeWithTerrainTexture(SourceTexture, adrNum);
        this.World = this.NewTerrainMesh.Terrain;
        Game.World = this.World;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        //Game.SimulationStep = Time.fixedDeltaTime;
        //	Debug.Log("active physics objects: " + physics.activePhysicsObjects);

        Profiler.BeginSample("Game.UpdateSimulation");

        //controls.FixedUpdate ();
        //CalculateWorld ();

        //Game.Physics.Update();

        Profiler.EndSample();
    }

	public void Explode(int posx, int posy, int radius)
    {
        SoundManager.instance.Play(SoundDef.ID_SOUND_INGAME_BTNCLICK);
        explode.explode(posx, posy, radius, 0f); //60);
        //explode.explode(posx, posy, 50, 0f); //60);
    }

    public void StartShootTimer()
    {
        StartCoroutine("ShootTimer");
    }

    IEnumerator ShootTimer()
    {
        if (!player.canShoot)
        {
            yield return new WaitForSeconds(0.15f);
            player.canShoot = true;
        }
    }
}
