using UnityEngine;
using System.Collections;

public class PlayerRenderer : MonoBehaviour
{
    public Vector2 Pos;

    public PixelDestruction Game;
    public CameraFollow FollowCamera;

    private Player player;

	void Start()
	{
	    if (Game != null)
	        player = Game.player;
	}

	void Update()
	{
        transform.position = new Vector2((int)player.Pos.x, (int)player.Pos.y);
		FollowCamera.TargetPosition = transform.position;
	}
}
