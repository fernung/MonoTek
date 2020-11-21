using Microsoft.Xna.Framework;
using MonoTek.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MonoTek.Graphics
{
    public interface IRender2d
    {
        IImage Target { get; set; }

        IPixel this[float x, float y] { get; set; }
        IPixel this[int x, int y] { get; set; }
        IPixel this[int index] { get; set; }

        void BlendPixel(int x, int y, IPixel c);
        void BlendPixel(int i, IPixel c);
        void DrawLine(Rectangle bounds, IPixel c);
        void DrawLine(Vector2 start, Vector2 end, IPixel c);
        void DrawLine(float x0, float y0, float x1, float y1, IPixel c);
        void DrawLine(int x0, int y0, int x1, int y1, IPixel c);
        void DrawTriangle(Vector2 v0, Vector2 v1, Vector2 v2, IPixel c);
        void FillTriangle(Vector2 v0, Vector2 v1, Vector2 v2, IPixel c);
        void RasterizeX(Vector2 p1, Vector2 p2, IPixel c);
        void RasterizeY(Vector2 p1, Vector2 p2, IPixel c);
    }
    public class Render2d : IRender2d
    {
        protected int[] _xBuffer, _yBuffer;
        protected IImage _target;

        public IImage Target 
        { 
            get => _target;
            set { _target = value; ResizeBuffers(); }
        }

        public IPixel this[float x, float y]
        {
            get => _target[x, y];
            set => _target[x, y] = value;
        }
        public IPixel this[int x, int y]
        {
            get => _target[x, y];
            set => _target[x, y] = value;
        }
        public IPixel this[int index]
        {
            get => _target[index];
            set => _target[index] = value;
        }

        public Render2d(int width, int height) : 
            this(Image.Create(width, height)) { }
        public Render2d(IImage target)
        {
            _target = target;
            ResizeBuffers();
        }

        public void BlendPixel(int x, int y, IPixel c) => BlendPixel(x + y * _target.Width, c);
        public void BlendPixel(int i, IPixel c) => this[i] = (Pixel)((c.Packed + this[i].Packed) >> 1);
        public void DrawLine(Rectangle bounds, IPixel c) => DrawLine(bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height, c);
        public void DrawLine(Vector2 start, Vector2 end, IPixel c) => DrawLine(start.X, start.Y, end.X, end.Y, c);
        public void DrawLine(float x0, float y0, float x1, float y1, IPixel c) => DrawLine((int)x0, (int)y0, (int)x1, (int)y1, c);
        public void DrawLine(int x0, int y0, int x1, int y1, IPixel c)
        {
            bool steep = Math.Abs(x0 - x1) < Math.Abs(y0 - y1);
            if (steep)
            {
                Algorithms.Swap(ref x0, ref y0);
                Algorithms.Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Algorithms.Swap(ref x0, ref x1);
                Algorithms.Swap(ref y0, ref y1);
            }
            int dx = x1 - x0, dy = y1 - y0;
            int x = x0, y = y0;
            int xInc = dx << 1, yInc = (y1 > y0 ? 1 : -1);
            int err = 0, err2 = Math.Abs(dy) << 1;
            if (steep) //Compiler optimization, cuts branching calls in half
            {
                for (; x <= x1; x++)
                {
                    this[y, x] = c;
                    err += err2;
                    if (err > dx)
                    {
                        y += yInc;
                        err -= xInc;
                    }
                }
            }
            else
            {
                for (; x <= x1; x++)
                {
                    this[x, y] = c;
                    err += err2;
                    if (err > dx)
                    {
                        y += yInc;
                        err -= xInc;
                    }
                }
            }
            _target.Redraw = true;
        }
        public void DrawTriangle(Vector2 v0, Vector2 v1, Vector2 v2, IPixel c)
        {
            if (v0.Y == v1.Y && v0.Y == v2.Y) return;
            if (v0.Y > v1.Y) Algorithms.Swap(ref v0, ref v1);
            if (v0.Y > v2.Y) Algorithms.Swap(ref v0, ref v2);
            if (v1.Y > v2.Y) Algorithms.Swap(ref v1, ref v2);
            DrawLine(v0, v1, c);
            DrawLine(v1, v2, c);
            DrawLine(v2, v0, c);
        }
        public void FillTriangle(Vector2 v0, Vector2 v1, Vector2 v2, IPixel c)
        {
            if (v0.Y == v1.Y && v0.Y == v2.Y) return;
            if (v0.Y > v1.Y) Algorithms.Swap(ref v0, ref v1);
            if (v0.Y > v2.Y) Algorithms.Swap(ref v0, ref v2);
            if (v1.Y > v2.Y) Algorithms.Swap(ref v1, ref v2);
            int totalHeight = (int)v2.Y - (int)v0.Y;
            bool secondHalf;
            int segHeight;
            float alpha, beta;
            Vector2 a, b;
            for (int i = 0; i < totalHeight; i++)
            {
                secondHalf = i > v1.Y - v0.Y || v1.Y == v0.Y;
                segHeight = secondHalf ? (int)v2.Y - (int)v1.Y : (int)v1.Y - (int)v0.Y;
                alpha = i / totalHeight;
                beta = (i - (secondHalf ? v1.Y - v0.Y : 0)) / (segHeight == 0 ? 1 : segHeight);
                a = v0 + (v2 - v0) * alpha;
                b = secondHalf ? v1 + (v2 - v1) * beta : v0 + (v1 - v0) * beta;
                if (a.X > b.X) Algorithms.Swap(ref a, ref b);
                for (int j = (int)a.X; j <= (int)b.X; j++)
                {
                    this[j, (int)v0.Y + i] = c;
                }
            }
            _target.Redraw = true;
        }
        public void DrawRectangle(Rectangle rect, IPixel c) => DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, c);
        public void DrawRectangle(Vector2 start, Vector2 end, IPixel c) => DrawRectangle((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, c);
        public void DrawRectangle(int x, int y, int width, int height, IPixel c)
        {
            if (width == 0 && height == 0) return;
            DrawLine(x, y, x + width, y, c);
            DrawLine(x + width, y, x + width, y + height, c);
            DrawLine(x + width, y + height, x, y + height, c);
            DrawLine(x, y + height, x, y, c);
        }
        public void FillRectangle(Rectangle rect, IPixel c) => FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, c);
        public void FillRectangle(Vector2 start, Vector2 end, IPixel c) => FillRectangle((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, c);
        public void FillRectangle(int x, int y, int width, int height, IPixel c)
        {
            int ix, iy, cx, cy;
            for (ix = 0; ix < width; ix++)
            {
                for (iy = 0; iy < height; iy++)
                {
                    cx = x + ix;
                    cy = y + iy;
                    if (cx < _target.Width && cy < _target.Height)
                        this[cx + (cy * _target.Width)] = c;
                }
            }
            _target.Redraw = true;
        }
        public void RasterizeX(Vector2 p1, Vector2 p2, IPixel c)
        {
            ResizeXBuffer();
            if (p1.Y > p2.Y) Algorithms.Swap(ref p1, ref p2);
            for (int y = (int)p1.Y; y <= p2.Y; y++)
            {
                double t = (y - p1.Y) / (float)(p2.Y - p1.Y);
                int x = (int)(p1.X * (1.0f - t) + p2.X * t + 0.5f);
                if (_xBuffer[y] < x)
                {
                    _yBuffer[y] = x;
                    this[0, y] = c;
                }
            }
            _target.Redraw = true;
        }
        public void RasterizeY(Vector2 p1, Vector2 p2, IPixel c)
        {
            ResizeYBuffer();
            if (p1.X > p2.X) Algorithms.Swap(ref p1, ref p2);
            for (int x = (int)p1.X; x <= p2.X; x++)
            {
                double t = (x - p1.X) / (float)(p2.X - p1.X);
                int y = (int)(p1.Y * (1.0f - t) + p2.Y * t + 0.5f);
                if (_yBuffer[x] < y)
                {
                    _yBuffer[x] = y;
                    this[x, 0] = c;
                }
            }
            _target.Redraw = true;
        }

        protected void ResizeXBuffer()
        {
            _xBuffer = new int[_target.Height];
            Array.Fill(_xBuffer, int.MinValue);
        }
        protected void ResizeYBuffer()
        {
            _yBuffer = new int[_target.Width];
            Array.Fill(_yBuffer, int.MinValue);
        }
        protected virtual void ResizeBuffers()
        {
            ResizeXBuffer();
            ResizeYBuffer();
        }
    }

    public interface IRender3d : IRender2d
    {
        int Depth { get; set; }
        IModel Model { get; set; }
        Vector3 Camera { get; set; }
        Vector3 LightDirection { get; set; }

        void DrawTriangle(Vector3 v0, Vector3 v1, Vector3 v2, IPixel c);
        void FillTriangle(Vector3 v0, Vector3 v1, Vector3 v2, IPixel c);
        void FillTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2, float intensity);
        void Wireframe(IModel model, float scale, IPixel c);
        void DrawModel(IModel model, float scale, IPixel c);
    }
    public class Render3d : Render2d, IRender3d
    {
        protected int _depth;
        protected float[] _zBuffer;
        protected IModel _model;
        protected Matrix _projection, _viewport;
        protected Vector3 _camera, _lightDirection;

        public int Depth { get => _depth; set => _depth = value; }
        public IModel Model { get => _model; set => _model = value; }
        public Vector3 Camera { get => _camera; set => _camera = value; }
        public Vector3 LightDirection { get => _lightDirection; set => _lightDirection = value; }

        public Render3d(int width, int height, string model) : 
            this(width, height, Graphics.Model.Load(model)) { }
        public Render3d(IImage targetImage, string model) : 
            this(targetImage, Graphics.Model.Load(model)) { }
        public Render3d(int width, int height, IModel model) : 
            this(Image.Create(width, height), model) { }
        public Render3d(IImage targetImage, IModel model) : base(targetImage)
        {
            _depth = 255;
            _model = model;
            _projection = Matrix.Identity;
            _viewport = CreateViewport
            (
                _target.Width << 3, 
                _target.Height << 3, 
                (_target.Width * 3) << 2, 
                (_target.Height * 3) << 2
            );
            _camera = new Vector3(0, 0, 3);
            _lightDirection = new Vector3(0, 0, -1);
            _projection.M43 = -1 / _camera.Z;
        }

        public void DrawTriangle(Vector3 v0, Vector3 v1, Vector3 v2, IPixel c) =>
            DrawTriangle(new Vector2(v0.X, v0.Y), new Vector2(v0.X, v0.Y), new Vector2(v0.X, v0.Y), c);
        public void FillTriangle(Vector3 v0, Vector3 v1, Vector3 v2, IPixel c)
        {
            ResizeZBuffer();
            Vector3 bboxmin, bboxmax, clamp, v, barycentricCoord;
            bboxmin = new Vector3(float.MaxValue, float.MaxValue, 0f);
            bboxmax = new Vector3(-float.MaxValue, -float.MaxValue, 0f);
            clamp = new Vector3((float)_target.Width - 1f, (float)_target.Height - 1f, 0f);
            bboxmin.X = (float)Math.Max(0.0, Math.Min(bboxmin.X, v0.X));
            bboxmin.X = (float)Math.Max(0.0, Math.Min(bboxmin.X, v1.X));
            bboxmin.X = (float)Math.Max(0.0, Math.Min(bboxmin.X, v2.X));
            bboxmin.Y = (float)Math.Max(0.0, Math.Min(bboxmin.Y, v0.Y));
            bboxmin.Y = (float)Math.Max(0.0, Math.Min(bboxmin.Y, v1.Y));
            bboxmin.Y = (float)Math.Max(0.0, Math.Min(bboxmin.Y, v2.Y));
            bboxmax.X = Math.Min(clamp.X, Math.Max(bboxmax.X, v0.X));
            bboxmax.X = Math.Min(clamp.X, Math.Max(bboxmax.X, v1.X));
            bboxmax.X = Math.Min(clamp.X, Math.Max(bboxmax.X, v2.X));
            bboxmax.Y = Math.Min(clamp.Y, Math.Max(bboxmax.Y, v0.Y));
            bboxmax.Y = Math.Min(clamp.Y, Math.Max(bboxmax.Y, v1.Y));
            bboxmax.Y = Math.Min(clamp.Y, Math.Max(bboxmax.Y, v2.Y));

            v = new Vector3();
            for (v.X = bboxmin.X; v.X <= bboxmax.X; v.X++)
            {
                for (v.Y = bboxmin.Y; v.Y <= bboxmax.Y; v.Y++)
                {
                    barycentricCoord = Algorithms.Barycentric(v0, v1, v2, v);
                    if (barycentricCoord.X < 0 || barycentricCoord.Y < 0 || barycentricCoord.Z < 0) continue;
                    v.Z = 0;
                    v.Z += v0.Z * barycentricCoord.X;
                    v.Z += v0.Z * barycentricCoord.Y;
                    v.Z += v0.Z * barycentricCoord.Z;
                    v.Z += v1.Z * barycentricCoord.X;
                    v.Z += v1.Z * barycentricCoord.Y;
                    v.Z += v1.Z * barycentricCoord.Z;
                    v.Z += v2.Z * barycentricCoord.X;
                    v.Z += v2.Z * barycentricCoord.Y;
                    v.Z += v2.Z * barycentricCoord.Z;
                    int idx = (int)(v.X + (v.Y * _target.Width));
                    if (_zBuffer[idx] < v.Z)
                    {
                        _zBuffer[idx] = v.Z;
                        this[(int)v.X, (int)v.Y] = c;
                    }
                }
            }
            _target.Redraw = true;
        }
        public void FillTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2, float intensity)
        {
            ResizeZBuffer();
            if (v0.Y == v1.Y && v0.Y == v2.Y) return;
            if (v0.Y > v1.Y) { Algorithms.Swap(ref v0, ref v1); Algorithms.Swap(ref uv0, ref uv1); }
            if (v0.Y > v2.Y) { Algorithms.Swap(ref v0, ref v2); Algorithms.Swap(ref uv0, ref uv2); }
            if (v1.Y > v2.Y) { Algorithms.Swap(ref v1, ref v2); Algorithms.Swap(ref uv1, ref uv2); }
            int totalHeight = (int)v2.Y - (int)v0.Y, segHeight, index;
            bool secondHalf;
            float alpha, beta;
            IPixel c;
            Vector3 a, b, p;
            Vector2 uvA, uvB, uvP;
            for (int i = 0; i < totalHeight; i++)
            {
                secondHalf = i > v1.Y - v0.Y || v1.Y == v0.Y;
                segHeight = secondHalf ? (int)v2.Y - (int)v1.Y : (int)v1.Y - (int)v0.Y;
                alpha = i / totalHeight;
                beta = (i - (secondHalf ? v1.Y - v0.Y : 0)) / (segHeight == 0 ? 1 : segHeight);
                a = v0 + (v2 - v0) * alpha;
                b = secondHalf ? v1 + (v2 - v1) * beta : v0 + (v1 - v0) * beta;
                uvA = uv0 + (uv2 - uv0) * alpha;
                uvB = secondHalf ? uv1 + (uv2 - uv1) * beta : uv0 + (uv1 - uv0) * beta;
                if (a.X > b.X) { Algorithms.Swap(ref a, ref b); Algorithms.Swap(ref uvA, ref uvB); }
                for (int j = (int)a.X; j <= (int)b.X; j++)
                {
                    float phi = b.X == a.X ? 1f : (float)(j - a.X) / (float)(b.X - a.X);
                    p = a + (b - a) * phi;
                    uvP = uvA + (uvB - uvA) * phi;
                    index = (int)(p.X + p.Y * _target.Width);
                    if(_zBuffer[index] < p.Z)
                    {
                        _zBuffer[index] = p.Z;
                        c = _model.Diffuse(uvP);
                        this[p.X, p.Y] = new Pixel
                        {
                            R = (byte)(c.R * intensity),
                            G = (byte)(c.G * intensity),
                            B = (byte)(c.B * intensity),
                            A = 255
                        };

                    }
                }
            }
            _target.Redraw = true;
        }
        public void Wireframe(IModel model, float scale, IPixel c)
        {
            foreach(var face in model.Faces)
            {
                DrawTriangle
                (
                    (model.Verticies[face[0].Vertex] + Vector3.One) * scale,
                    (model.Verticies[face[1].Vertex] + Vector3.One) * scale,
                    (model.Verticies[face[2].Vertex] + Vector3.One) * scale,
                    c
                );
            }
        }
        public void DrawModel(IModel model, float scale, IPixel c)
        {
            //Setup Viewport, Projection
            int i;
            float intensity;
            Vector3 normal, v;
            Vector2[] uv = new Vector2[3];
            Vector3[] screen = new Vector3[3];
            Vector3[] world = new Vector3[3];
            foreach (var face in model.Faces)
            {
                for (i = 0; i < 3; i++)
                {
                    v = model.Verticies[face[i].Vertex];
                    screen[i] = (_viewport * _projection * v.ToMatrix()).ToVector3();
                    world[i] = v;
                }
                normal = Vector3.Cross(world[2] - world[0], world[1] - world[0]);
                normal.Normalize();
                intensity = Vector3.Dot(normal, _lightDirection);
                if(intensity > 0)
                {
                    for(i = 0; i < 3; i++)
                    {
                        uv[i] = model.UV(face, i);
                    }

                    FillTriangle
                    (
                        screen[0],
                        screen[1],
                        screen[2],
                        uv[0],
                        uv[1],
                        uv[2],
                        intensity
                    );
                }
            }
        }

        protected Matrix CreateViewport(int x, int y, int w, int h)
        {
            var m = Matrix.Identity;
            m.M14 = x + (w << 1);
            m.M24 = y + (h << 1);
            m.M34 = _depth << 1;
            m.M11 = w << 1;
            m.M22 = h << 1;
            m.M33 = _depth << 1;
            return m;
        }
        protected void ResizeZBuffer()
        {
            _zBuffer = new float[_target.Size];
            Array.Fill(_zBuffer, float.MinValue);
        }
        protected override void ResizeBuffers()
        {
            base.ResizeBuffers();
            ResizeZBuffer();
        }
    }
}
