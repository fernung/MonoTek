using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoTek.Core
{
    public class GameClient : Game
    {
        private static GameClient _instance;
        public static GameClient Instance
        {
            get { if (_instance == null) _instance = new GameClient(); return _instance; }
        }

        private bool _running;
        private Random _random;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _blankTexture;
        private RenderTarget2D _offscreen;
        private SpriteFont _font;
        private float _aspect;
        private Point _oldSize;

        public bool Running { get => _running; set => _running = value; }
        public Random Random => _random;
        public GraphicsDeviceManager Graphics => _graphics;
        public SpriteBatch SpriteBatch => _spriteBatch;
        public SpriteFont DefaultFont => _font;

        public GameClient()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(ClientSizeChanged);
        }

        protected override void Initialize()
        {
            _running = true;
            _random = new Random();
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _aspect = GraphicsDevice.Viewport.AspectRatio;
            _oldSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blankTexture.SetData(new Color[] { Color.FromNonPremultiplied(255, 255, 255, 255) });
            _offscreen = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("DefaultFont");
            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            if (_offscreen != null) _offscreen.Dispose();
            if (_blankTexture != null) _blankTexture.Dispose();
            if (_spriteBatch != null) _spriteBatch.Dispose();
            base.UnloadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            if (!_running) Exit();

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(_offscreen);
            return base.BeginDraw();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        protected override void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_offscreen, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
            base.EndDraw();
        }

        private void ClientSizeChanged(object sender, EventArgs e)
        {
            Window.ClientSizeChanged -= new EventHandler<EventArgs>(ClientSizeChanged);
            if (Window.ClientBounds.Width != _oldSize.X)
            {
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / _aspect);
            }
            else if (Window.ClientBounds.Height != _oldSize.Y)
            {
                _graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * _aspect);
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            }
            _graphics.ApplyChanges();
            _oldSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            Window.ClientSizeChanged += new EventHandler<EventArgs>(ClientSizeChanged);
        }
    }
}
