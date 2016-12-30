using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathUtil;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Collision
    {
        public enum CollisionLayer
        {
            Terrain,
            Player,
            Bullet,
        }

        public struct HitResult
        {
            public bool IsHit { get; set; }
            public Vector2 HitVector { get; set; }
            public CollisionLayer Layer { get; set; }
            public WorldObject ObjectHit { get; set; }
        }

        // Collide bounding box with terrain based on current movement: only process the directions we are moving forward in
        public static HitResult CollideBoundingBoxWithTerrain(BoundingBox bounds, Vector2 velocity)
        {
            var intMinBounds = new Vector2I(bounds.Min.x, bounds.Min.y);
            var intMaxBounds = new Vector2I(bounds.Max.x, bounds.Max.y);
            var hitDirection = new Vector2();

            for (int x = intMinBounds.x; x < intMaxBounds.x; x++)
            {
                if (velocity.y < 0 && Game.World.IsSolid(new Vector2I(x, intMinBounds.y)))
                    hitDirection.y = -1f;
                if (velocity.y > 0 && Game.World.IsSolid(new Vector2I(x, intMaxBounds.y - 1)))
                    hitDirection.y = +1f;
            }

            for (int y = intMinBounds.y; y <= intMinBounds.y; y++)
            {
                if (velocity.x < 0 && Game.World.IsSolid(new Vector2I(intMinBounds.x, y)))
                    hitDirection.x = -1f;
                if (velocity.x > 0 && Game.World.IsSolid(new Vector2I(intMaxBounds.x - 1, y)))
                    hitDirection.x = +1f;
            }

            if (hitDirection != Vector2.zero)
                return new HitResult { IsHit = true, HitVector = hitDirection.normalized, Layer = CollisionLayer.Terrain };
            else
                return new HitResult();
        }

        // Using per-pixel collision computation - we backtrack our movement to find the first non-colliding position we passed this frame
        public static HitResult CollideMovingObjectWithTerrain(MovableObject obj)
        {
            Profiler.BeginSample("Collision.CollideMovingObjectWithTerrain");
            if (obj.Velocity == Vector2.zero)
                return new HitResult();

            var lastHit = new HitResult();

            // Do a simple bounding box collision for now
            var hit = CollideBoundingBoxWithTerrain(obj.Bounds, obj.Velocity);
            if (hit.IsHit)
            {
                // If we collided with the terrain, we now try to backtrack the last movement pixel-by-pixel 
                //  and find the first possible position in which we are not colliding
                Vector2 distance = obj.Velocity * Game.SimulationStep;
                float distXtoY = Math.Abs(distance.y != 0 ? (distance.x / distance.y) : Single.PositiveInfinity);

                // Now we backtrack our movement
                var backtrackDist = new Vector2();
                var originalHit = hit;

                // Unless we travelled all the way to our original position in previous frame, keep testing
                //  positions in between for collision and try to find a collision-free location
                while (hit.IsHit && Math.Abs(backtrackDist.x) <= Math.Abs(distance.x) && Math.Abs(backtrackDist.y) <= Math.Abs(distance.y))
                {
                    lastHit = hit;
                    if (Math.Abs(backtrackDist.x) == Math.Abs(distance.x) && Math.Abs(backtrackDist.y) == Math.Abs(distance.y))
                        break;
                    if (Math.Abs(backtrackDist.x) > Math.Abs(-distance.x))
                        UnityEngine.Debug.LogWarning("The math does not compute.");
                    if (Math.Abs(backtrackDist.y) > Math.Abs(-distance.y))
                        UnityEngine.Debug.LogWarning("The math does not compute.");

                    Vector2 backtrackDelta;
                    // Try to follow in integer math (we deal with pixels) the distance vector
                    //  Normalize both and try to find the axis with greater difference
                    float backDistXtoY = Math.Abs(backtrackDist.y != 0 ? (backtrackDist.x / backtrackDist.y) : Single.PositiveInfinity);
                    if (distXtoY > backDistXtoY)
                    {
                        backtrackDelta = new Vector2(-Math.Sign(distance.x), 0);
                    }
                    else
                    {
                        backtrackDelta = new Vector2(0, -Math.Sign(distance.y));
                    }

                    // Check if we are not exceeding the original location, we want to stop there.
                    if (Math.Abs(backtrackDist.x + backtrackDelta.x) > Math.Abs(distance.x))
                        backtrackDelta.x = -Math.Sign(distance.x) * (Math.Abs(distance.x) - Math.Abs(backtrackDist.x));
                    if (Math.Abs(backtrackDist.y + backtrackDelta.y) > Math.Abs(distance.y))
                        backtrackDelta.y = -Math.Sign(distance.y) * (Math.Abs(distance.y) - Math.Abs(backtrackDist.y));

                    backtrackDist += backtrackDelta;
                    obj.Pos += backtrackDelta;
                    obj.UpdateBoundingBox();
                    hit = CollideBoundingBoxWithTerrain(obj.Bounds, obj.Velocity);
                }

                // That's it, we're no longer moving!
                if (originalHit.HitVector.x > 0)
                    obj.Velocity.x = 0;
                if (originalHit.HitVector.x < 0)
                    obj.Velocity.x = 0;
                if (originalHit.HitVector.y > 0)
                    obj.Velocity.y = 0;
                if (originalHit.HitVector.y < 0)
                    obj.Velocity.y = 0;
            }
            Profiler.EndSample();
            return lastHit;
        }
    }
}
