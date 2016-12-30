using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathUtil;
using UnityEngine;

namespace Assets.Scripts
{
    public static partial class Game
    {
        public static class Terrain
        {
            public static bool IsSolid(int x, int y)
            {
                if (Game.World.IsInside(new Vector2I(x, y)))
                {
                    var pixel = Game.World.TerrainLayer.GetAt(new Vector2I(x, y));
                    return pixel.a == 255;
                }
                return true; // border IS solid
            }

            public static void AddTerrain(int x, int y, Color c)
            {
                if (Game.World.IsInside(new Vector2I(x, y)))
                {
                    Game.World.ChangeTerrainColorAt(new Vector2I(x, y), c);
                }
                else
                    Debug.LogWarning("AddTerrain outside bounds");
            }

            public static void RemoveTerrain(int x, int y)
            {
                if (Game.World.IsInside(new Vector2I(x, y)))
                {
                    Game.World.ChangeTerrainAlphaAt(new Vector2I(x, y), 0);
                }
                else
                    Debug.LogWarning("RemoveTerrain outside bounds");
            }

            public static Color GetTerrain(int x, int y)
            {
                if (Game.World.IsInside(new Vector2I(x, y)))
                    return Game.World.TerrainLayer.GetAt(new Vector2I(x, y));
                return Color.white;
            }

            public static Vector2 GetNormal(int x, int y)
            {
                // First find all nearby solid pixels, and create a vector to the average solid pixel from (x,y)
                float avgX = 0;
                float avgY = 0;
                for (int w = -3; w <= 3; w++)
                {
                    for (int h = -3; h <= 3; h++)
                    {
                        if (IsSolid(x + w, y + h))
                        {
                            avgX -= w;
                            avgY -= h;
                        }
                    }
                }
                float len = Mathf.Sqrt(avgX*avgX + avgY*avgY); // get the distance from (x,y)
                if (len == 0)
                {
                    return Vector2.zero;
                }
                return new Vector2(avgX/len, avgY/len); // normalize the vector by dividing by that distance
            }
        }
    }
}
