using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MathUtil;
using UnityEngine;

namespace Assets.Scripts
{
    // Represents the world terrain data
    public class World : ICollidable
    {
        public BoundingBox Bounds;
        
        public TerrainLayer TerrainLayer { get; private set; }
        public DynamicLayer DynamicLayer { get; private set; }

        public TerrainMesh Mesh { get; private set; }

        public World(Vector2I origin, Vector2I dimensions, TerrainMesh mesh, Texture2D texture = null)
        {
    
            // Collide with out-of-bounds world
            this.Bounds = new BoundingBox
                          {
                              Min = (Vector2)origin,
                              Max = (Vector2)(origin + dimensions),
                              Size = (Vector2)(dimensions)
                          };
            
            
            this.Mesh = mesh;
            this.TerrainLayer = new TerrainLayer(origin, dimensions, texture);
            this.DynamicLayer = new DynamicLayer();
        }

        public bool IsInside(Vector2 coord)
        {
            return coord.x >= this.Bounds.Min.x && coord.x < this.Bounds.Max.x && coord.y >= this.Bounds.Min.y && coord.y < this.Bounds.Max.y;
        }
        public bool IsInside(Vector2I coord)
        {
            return IsInside((Vector2) coord);
        }

        public bool IsSolid(Vector2I coord)
        {
            if (!IsInside(coord))
                return true;
            return this.TerrainLayer.GetAt(coord).a != 0;
        }
        public bool IsSolid(Vector2 coord)
        {
            if (!IsInside(coord))
                return true;
            return this.TerrainLayer.GetAt((Vector2I)coord).a != 0;
        }

        public Color32 GetPixelAt(Vector2 coord)
        {
            return GetPixelAt((Vector2I) coord);
        }
        public Color32 GetPixelAt(Vector2I coord)
        {
            Color32 dynamicPixel = this.DynamicLayer.GetAt(coord);
            if (dynamicPixel.a > 0)
            {
                return dynamicPixel;
            }
            var color = this.TerrainLayer.GetAt(coord);
            return color;
        }

        public IEnumerable<Vector2I> EnumerateChangeList()
        {
            foreach (var terrainPixel in this.TerrainLayer.ChangeList)
            {
                yield return terrainPixel;
            }
            foreach (var dynamicPixel in this.DynamicLayer.ChangeList)
            {
                yield return dynamicPixel;
            }
        }

        public void FlushChangeList()
        {
            this.TerrainLayer.ChangeList.Clear();
            this.DynamicLayer.ChangeList.Clear();
        }

        // All changes are queued and committed on next UpdateSimulation
        public void ChangeTerrainColorAt(Vector2I position, Color32 newColorData)
        {
            this.TerrainLayer.SetAt(position, newColorData);
        }

        public void ChangeTerrainAlphaAt(Vector2I position, float newAlpha)
        {
            this.TerrainLayer.SetAlphaAt(position, newAlpha);
        }
        public DynamicLayerEntry CreateDecal(Vector3I position, Color32 color)
        {
            return this.DynamicLayer.CreateDynamicEntry(position, color);
        }
        public void RemoveDecal(DynamicLayerEntry dynamicEntry)
        {
            this.DynamicLayer.RemoveDynamicEntry(dynamicEntry);
        }

        public BoundingBox GetBounds()
        {
            return this.GetBounds();
        }

        public Collision.HitResult ProcessRaycast(Ray2D ray, float width)
        {
            return new Collision.HitResult();
        }

    }

    // Contains the all pixels of the terrain layer
    public class TerrainLayer
    {
        public Color32[] Data { get; private set; }
        public Vector2I Origin { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public List<Vector2I> ChangeList { get; private set; }

        public TerrainLayer(Vector2I origin, Vector2I dimensions, Texture2D initialTexture = null)
        {
            this.Origin = origin;
            this.Width = dimensions.x;
            this.Height = dimensions.y;
            this.Data = new Color32[Width * Height];
            this.ChangeList = new List<Vector2I>();
            if (initialTexture != null)
            {
                if (initialTexture.width != this.Width || initialTexture.height != this.Height)
                {
                    // Dimensions don't match, we will repeat the texture over and over
                    var pixels = initialTexture.GetPixels32();
                    for (int y = 0; y < this.Height; ++y)
                    {
                        int xRemain = this.Width;
                        while (xRemain > 0)
                        {
                            Array.Copy(pixels, (y % initialTexture.height)*initialTexture.width, this.Data, y*this.Width + this.Width - xRemain, Math.Min(xRemain, initialTexture.width));
                            xRemain -= initialTexture.width;
                        }
                    }
                }
                else
                    Array.Copy(initialTexture.GetPixels32(), this.Data, Width * Height);
            }
        }

        public Color32 GetAt(Vector2I pos)
        {
            return this.Data[DataArrayIndex(pos)];
        }

        public void SetAt(Vector2I pos, Color32 value)
        {
            this.Data[DataArrayIndex(pos)] = value;
            this.ChangeList.Add(pos);
        }

        public Color32 SetAlphaAt(Vector2I pos, float alpha)
        {
            var value = GetAt(pos);
            value.a = (byte)(255*alpha);
            SetAt(pos, value);
            return value;
        }

        private int DataArrayIndex(Vector2I pos)
        {
            return pos.x - Origin.x + ((pos.y - Origin.y))*this.Width;
        }
    }
}
