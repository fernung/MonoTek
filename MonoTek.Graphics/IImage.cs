using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoTek.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTek.Graphics
{
    public interface IImage : IDisposable
    {
        bool Redraw { get; set; }
        int Width { get; }
        int Height { get; }
        int Size { get; }
        Texture2D Texture { get; }
        Rectangle Bounds { get; }
        IEnumerable<IPixel> Pixels { get; }
        IPixel this[float x, float y] { get; set; }
        IPixel this[int x, int y] { get; set; }
        IPixel this[int index] { get; set; }

        void Clear();
        void Fill(IPixel c);
        void FlipBoth();
        void FlipHorizontal();
        void FlipVertical();
    }

    public class Image : IImage
    {
        protected bool _disposed, _redraw;
        protected int _width, _height, _size;
        protected IPixel[] _pixels;
        protected Texture2D _texture;
        protected Rectangle _bounds;

        public bool Redraw { get => _redraw; set => _redraw = value; }
        public int Width => _width;
        public int Height => _height;
        public int Size => _size;
        public Texture2D Texture
        {
            get
            {
                if (_redraw)
                {
                    _texture.SetData<int>(_pixels.ToPacked().ToArray(), 0, _size);
                    _redraw = false;
                }
                return _texture;
            }
        }
        public Rectangle Bounds => _bounds;
        public IEnumerable<IPixel> Pixels => _pixels;

        public IPixel this[float x, float y]
        {
            get => this[(int)(x * _width), (int)(y * _height)];
            set { this[(int)(x * _width), (int)(y * _height)] = value; _redraw = true; }
        }
        public IPixel this[int x, int y]
        {
            get => this[x + y * _width];
            set { this[x + y * _width] = value; _redraw = true; }
        }
        public IPixel this[int index]
        {
            get => _pixels[index % _size];
            set { _pixels[index % _size] = value; _redraw = true; }
        }

        protected Image(IImage other) : this(other.Width, other.Height, other.Pixels) { }
        protected Image(string filename) 
        {
            var texture = GameClient.Instance.Content.Load<Texture2D>(filename);

            _disposed = false;
            _redraw = false;
            _width = texture.Width;
            _height = texture.Height;
            _size = _width * _height;
            _bounds = new Rectangle(0, 0, _width, _height);
            _pixels = new IPixel[_size];
            _texture = new Texture2D(GameClient.Instance.GraphicsDevice, _width, _height, false, SurfaceFormat.Color);
            _texture.SetData<int>(_pixels.ToPacked().ToArray(), 0, _size);
            byte[] pixels = new byte[_size];
            texture.GetData(pixels);
            Buffer.BlockCopy(pixels, 0, _pixels, 0, _size);
        }
        protected Image(int width, int height, IEnumerable<IPixel> pixels) : 
            this(width, height) 
        {
            Buffer.BlockCopy(pixels.ToArray(), 0, _pixels, 0, _size);
        }
        protected Image(int width, int height)
        {
            _disposed = false;
            _redraw = false;
            _width = width;
            _height = height;
            _size = width * height;
            _bounds = new Rectangle(0, 0, width, height);
            _pixels = new IPixel[_size];
            _texture = new Texture2D(GameClient.Instance.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            _texture.SetData<int>(_pixels.ToPacked().ToArray(), 0, _size);
        }

        public void Clear()
        {
            _redraw = true;
            _pixels = new IPixel[_size];
        }
        public void Fill(IPixel c) => Array.Fill(_pixels, c);
        public void FlipBoth() { Array.Reverse(_pixels); _redraw = true; }
        public void FlipHorizontal()
        {
            int i, j, w1 = _width - 1, w2 = _width >> 1;
            IPixel c1, c2;
            for (i = 0; i < w2; i++)
            {
                for (j = 0; j < _height; j++)
                {
                    c1 = this[i, j];
                    c2 = this[w1 - i, j];
                    this[i, j] = c2;
                    this[w1 - i, j] = c1;
                }
            }
            _redraw = true;
        }
        public void FlipVertical() { FlipBoth(); FlipHorizontal(); }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _texture.Dispose();
        }

        public static IImage Copy(IImage other) => new Image(other);
        public static IImage Load(string filename) => new Image(filename);
        public static IImage Create(int width, int height) => new Image(width, height);
        public static IImage Create(int width, int height, IEnumerable<IPixel> pixels) => new Image(width, height, pixels);
    }
}
