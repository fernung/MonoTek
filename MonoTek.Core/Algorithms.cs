using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoTek.Core
{
    public static class Algorithms
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        //public static Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        //{
        //    Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
        //    double d00 = Vector3.DotProduct(v0, v0);
        //    double d01 = Vector3.DotProduct(v0, v1);
        //    double d11 = Vector3.DotProduct(v1, v1);
        //    double d20 = Vector3.DotProduct(v2, v0);
        //    double d21 = Vector3.DotProduct(v2, v1);
        //    double demon = d00 * d11 - d01 * d01;
        //    double u, v, w;
        //    v = (d11 * d20 - d01 * d21) / demon;
        //    w = (d00 * d21 - d01 * d20) / demon;
        //    u = 1.0 - v - w;
        //    return new Vector3(u, v, w);
        //}
        public static Vector3 Barycentric(Vector3 pt0, Vector3 pt1, Vector3 pt2, Vector3 p)
        {
            Vector3 u = Vector3.Cross(new Vector3(pt2.X - pt0.X, pt1.X - pt0.X, pt0.X - p.X),
                                        new Vector3(pt2.Y - pt0.Y, pt1.Y - pt0.Y, pt0.Y - p.Y));
            if (Math.Abs((int)u.Z) < 1) return new Vector3(-1, 1, 1);
            return new Vector3(1.0f - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
        }
        public static Vector3 WorldToScreen(Vector3 v, float width, float height)
        {
            return new Vector3((int)((v.X + 1.0f) * width / 2 + 0.5f), (int)((v.Y + 1.0f) * height / 2 + 0.5f), v.Z);
        }
        public static Vector3 WorldToScreen(Vector3 v, int w, int h)
        {
            return new Vector3((int)((v.X + 1.0f) * w / 2 + 0.5f), (int)((v.Y + 1.0f) * h / 2 + 0.5f), v.Z);
        
        
        }
    }

    public static class AlgorithmExtensions
    {
        public static Vector3 ToVector3(this Matrix m) => 
            new Vector3(m.M11 / m.M41, m.M21 / m.M41, m.M31 / m.M41);
        public static Matrix ToMatrix(this Vector3 v) => new Matrix
        (
            v.X,  0, 0, 0, 
            v.Y,  0, 0, 0, 
            v.Z,  0, 0, 0, 
            1.0f, 0, 0, 0
        );
    }
}
