using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathUtil;
using UnityEngine;

namespace Assets.Scripts
{
    public class CollidableObject : MovableObject
    {

    }

    public class MovableObject : WorldObject
    {
        public Vector2 Velocity;

        public override void UpdateSimulation()
        {
            var moveVec = this.Velocity*Game.SimulationStep;
            this.Pos += moveVec;

            UpdateBoundingBox();
        }

        public void UpdateBoundingBox()
        {
            this.Bounds.Min = this.Pos - 0.5f * this.Bounds.Size;
            this.Bounds.Max = this.Pos + 0.5f * this.Bounds.Size;
        }

        public virtual void ResolveCollision()
        {
            var hitResult = Collision.CollideMovingObjectWithTerrain(this);
            if (hitResult.IsHit)
                OnCollided(hitResult);
        }

        protected virtual void OnCollided(Collision.HitResult hit)
        {
            
        }
    }

    public class WorldObject : ICollidable
    {
        public Vector2 Pos;
        public BoundingBox Bounds;

        public virtual void UpdateSimulation()
        {
            
        }

        public virtual void UpdateVisual(float deltaTime)
        {

        }

        public virtual void FrameBegin()
        {

        }

        public virtual void FrameEnd()
        {

        }

        #region ICollidable
        public BoundingBox GetBounds()
        {
            return Bounds;
        }

        public Collision.HitResult ProcessRaycast(Ray2D ray, float width)
        {
            return new Collision.HitResult {IsHit = false};
        }
        #endregion
    }

    internal interface ICollidable
    {
        BoundingBox GetBounds();
        Collision.HitResult ProcessRaycast(Ray2D ray, float width);
    }

    public struct BoundingBox
    {
        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }
        public Vector2 Size { get; set; }

        public static BoundingBox operator +(BoundingBox box, Vector2 vec)
        {
            return new BoundingBox
                   {
                       Max = box.Max + vec,
                       Min = box.Min + vec,
                       Size = box.Size
                   };
        }
    }

}
