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
        // constants
        float ballMass;
        float userForce;
        float gravity;
        float pixelToMeter;
        float frictionCoefficient;
        float maxSpeed;
        float bounceFactor; // how much of the initial force is maintained in the follow-up bounce

        // variable
        bool onFloor;
        int prevFloorContact;
        float ballRotation;
        Vector2 ballPosition;
        Vector2 ballVelocity;

        // sprites
        Texture2D ballTexture;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ResolutionIndependentRenderer _resolutionIndependence;
        private Camera2D _camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // game constants and variables
            pixelToMeter = 200f;
            ballMass = 1f;
            userForce = 400f;
            bounceFactor = 0.5f;
            frictionCoefficient = 0.05f;
            maxSpeed = 10f * pixelToMeter;
            gravity = -9.81f * pixelToMeter;
            onFloor = false;
            prevFloorContact = 0;
            ballRotation = 0f;
            ballVelocity = Vector2.Zero;
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

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

            // TODO: use this.Content to load your game content here
            ballTexture = Content.Load<Texture2D>("ball");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();

            // user actions
            if (kstate.IsKeyDown(Keys.Up) && onFloor)
            {
                prevFloorContact = 0;
                ballVelocity.Y -= ((userForce / ballMass) * pixelToMeter) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Left))
                ballVelocity.X -= (userForce / ballMass) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Right))
                ballVelocity.X += (userForce / ballMass) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // limit velocity to maxSpeed
            if (ballVelocity.X < -maxSpeed || ballVelocity.X > maxSpeed)
            {
                ballVelocity.X = Math.Sign(ballVelocity.X) * maxSpeed;
            }
            if (ballVelocity.Y < -maxSpeed || ballVelocity.Y > maxSpeed)
            {
                ballVelocity.Y = Math.Sign(ballVelocity.Y) * maxSpeed;
            }

            // resistence
            if (!onFloor)
            {
                ballVelocity.Y -= gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                ballVelocity.X = ballVelocity.X * (1 - frictionCoefficient);
                // bouce - floor
                if (prevFloorContact != 0)
                {
                    ballVelocity.Y -= (userForce / ballMass) * pixelToMeter * (float)Math.Pow(bounceFactor, prevFloorContact) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            // bounce - x boundaries
            if (ballPosition.X >= _graphics.PreferredBackBufferWidth - ballTexture.Width / 2)
            {
                ballVelocity.X = -1 * (userForce / ballMass) * pixelToMeter * bounceFactor * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (ballPosition.X <= ballTexture.Width / 2)
            {
                ballVelocity.X = (userForce / ballMass) * pixelToMeter * bounceFactor * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // calc positions
            ballPosition.X += ballVelocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ballPosition.Y += ballVelocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // calc rotation
            if (ballVelocity.X != 0)
            {
                ballRotation += 1f * (ballVelocity.X / maxSpeed);
            }
            else
            {
                ballRotation = 0f;
            }

            // boundaries
            if (ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2)
            {
                ballPosition.X = _graphics.PreferredBackBufferWidth - ballTexture.Width / 2;
            }
            else if (ballPosition.X < ballTexture.Width / 2)
            {
                ballPosition.X = ballTexture.Width / 2;
            }

            if (ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height / 2)
            {
                prevFloorContact += 1;
                onFloor = true;
                ballVelocity.Y = 0;
                ballPosition.Y = _graphics.PreferredBackBufferHeight - ballTexture.Height / 2;
            }
            else if (ballPosition.Y <= ballTexture.Height / 2)
            {
                ballVelocity.Y = 0;
                ballPosition.Y = ballTexture.Height / 2;
            }
            else
            {
                onFloor = false;
            }

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
            _spriteBatch.Draw(
                ballTexture,
                ballPosition,
                null,
                Color.White,
                ballRotation,
                new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
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
