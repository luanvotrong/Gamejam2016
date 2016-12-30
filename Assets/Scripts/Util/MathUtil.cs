using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MathUtil
{
    public struct Vector2I : IEquatable<Vector2I>
    {
        public int x;
        public int y;

        public Vector2I(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2I(float x, float y)
        {
            // Compensate for the fast that we would get -0.99 to +0.99 as zero, making this value covering twice as much floats
            this.x = (int)(x + 0.5f);
            this.y = (int)(y + 0.5f);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2I && Equals((Vector2I) obj);
        }
        public bool Equals(Vector2I other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        #endregion


        static public Vector2I operator +(Vector2I first, Vector2I second)
        {
            return new Vector2I(first.x + second.x, first.y + second.y);
        }
        static public Vector2I operator -(Vector2I first, Vector2I second)
        {
            return new Vector2I(first.x - second.x, first.y - second.y);
        }
        public static explicit operator Vector2(Vector2I source)
        {
            return new Vector2(source.x - 0.5f, source.y - 0.5f);
        }
        public static explicit operator Vector2I(Vector2 source)
        {
            return new Vector2I((int)(source.x + 0.5f), (int)(source.y + 0.5f));
        }
    }

    public struct Vector3I : IEquatable<Vector3I>
    {
        public int x;
        public int y;
        public int z;

        public Vector3I(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        static public Vector3I operator +(Vector3I first, Vector3I second)
        {
            return new Vector3I(first.x + second.x, first.y + second.y, first.z /* do not Add Z, it stays! */);
        }

        public Vector2I ToVector2I()
        {
            return new Vector2I(x, y);
        }

        #region Equals 

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3I && Equals((Vector3I) obj);
        }

        public bool Equals(Vector3I other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ z;
                return hashCode;
            }
        }

        #endregion

    }
}


