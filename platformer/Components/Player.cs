using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace platformer
{
    class Player
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

        private readonly Game _game;
        private SpriteBatch _spriteBatch;
        private readonly GraphicsDeviceManager _graphics;

        public Player(Game game, GraphicsDeviceManager graphics, Vector2 ballPosition)
        {
            this._game = game;
            this._graphics = graphics;

            // game constants and variables
            this.pixelToMeter = 200f;
            this.ballMass = 1f;
            this.userForce = 400f;
            this.bounceFactor = 0.5f;
            this.frictionCoefficient = 0.05f;
            this.maxSpeed = 10f * pixelToMeter;
            this.gravity = -9.81f * pixelToMeter;
            this.onFloor = false;
            this.prevFloorContact = 0;
            this.ballRotation = 0f;
            this.ballVelocity = Vector2.Zero;
            this.ballPosition = ballPosition;
        }

        public void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            ballTexture = _game.Content.Load<Texture2D>("ball");
        }

        public void Update(GameTime gameTime)
        {
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
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            // TODO: Add your drawing code here
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
        }
    }
}
