using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rendering;
using System;
using System.Diagnostics;

namespace platformer
{
    public class Game1 : Game
    {
        Vector2 ballPosition;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ResolutionIndependentRenderer _resolutionIndependence;
        private Camera2D _camera;
        private Player player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            player = new Player(this, _graphics, ballPosition);
        }

        protected override void Initialize()
        {
            // system settings
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width - 100;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height - 100;
            //_graphics.IsFullScreen = true;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _resolutionIndependence = new ResolutionIndependentRenderer(this);
            _camera = new Camera2D(_resolutionIndependence);
            InitializeResolutionIndependence(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);

            _camera.Zoom = 1f;
            _camera.Position = new Vector2(_resolutionIndependence.VirtualWidth / 2, _resolutionIndependence.VirtualHeight / 2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            player.LoadContent();

            this.Content.RootDirectory = "Content";
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _resolutionIndependence.BeginDraw();
            //_spriteBatch.Begin();
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearWrap,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                _camera.GetViewTransformationMatrix()
            );
            player.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitializeResolutionIndependence(int realScreenWidth, int realScreenHeight)
        {
            _resolutionIndependence.VirtualWidth = _resolutionIndependence.ScreenWidth = realScreenWidth;
            _resolutionIndependence.VirtualHeight = _resolutionIndependence.ScreenHeight = realScreenHeight;
            _resolutionIndependence.Initialize();

            _camera.RecalculateTransformationMatrices();
        }
    }
}
