using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Pong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D imageLigne;
        private KeyboardState keyboard;
        private Vector2 raquetteVelocity;
        private int scoreLeft;
        private int scoreRight;
        private float dt;
        private const int SCOREMAX = 5;
        private bool gameOver;
        private Song musique;
        private SoundEffect pingEffect;
        private SoundEffect bangEffect;
        private struct Raquette_t
        {
            public Vector2 position;
            public Texture2D image;
        };
        private struct Balle_t
        {
            public Vector2 position;
            public Vector2 velocity;
            public Texture2D image;
        };
        Balle_t balle;
        Raquette_t raquetteLeft;
        Raquette_t raquetteRight;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void Reset()
        {
            raquetteLeft.position.Y = raquetteLeft.image.Height / 2 + 10;
            raquetteRight.position.Y = raquetteRight.position.Y = raquetteRight.image.Height / 2 + 10;

            balle.position.X = _graphics.PreferredBackBufferWidth / 2;
            balle.position.Y = _graphics.PreferredBackBufferHeight / 2;
        }

        private void endGame()
        {
            balle.velocity = new Vector2(0, 0);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 1040;
            _graphics.ApplyChanges();

            keyboard = Keyboard.GetState();

            balle.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            balle.velocity = new Vector2(4, 4);

            raquetteLeft.position = new Vector2(0, _graphics.PreferredBackBufferHeight / 2);
            raquetteRight.position = new Vector2(0, _graphics.PreferredBackBufferHeight / 2);

            raquetteVelocity = new Vector2(0, 8);

            scoreLeft = 0;
            scoreRight = 0;
            dt = 0f;

            gameOver = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            imageLigne = Content.Load<Texture2D>("barre");

            balle.image = Content.Load<Texture2D>("balle");

            raquetteLeft.image = Content.Load<Texture2D>("raquette");
            raquetteRight.image = Content.Load<Texture2D>("raquette");

            musique = Content.Load<Song>("music");
            pingEffect = Content.Load<SoundEffect>("ping");
            bangEffect = Content.Load<SoundEffect>("bang");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(musique);
            MediaPlayer.Volume = 0.3f;

            Reset();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            dt += (float)gameTime.ElapsedGameTime.TotalSeconds;

            keyboard = Keyboard.GetState();

            raquetteLeft.position.X = _graphics.PreferredBackBufferWidth - raquetteLeft.image.Width;
            raquetteRight.position.X = raquetteRight.image.Width;

            // TODO: Add your update logic here

            if (keyboard.IsKeyDown(Keys.Up) && raquetteLeft.position.Y > raquetteLeft.image.Height / 2 + 8)
            {
                raquetteLeft.position -= raquetteVelocity;
            }
            else if (keyboard.IsKeyDown(Keys.Down) && raquetteLeft.position.Y < _graphics.PreferredBackBufferHeight - raquetteLeft.image.Height / 2 - 8)
            {
                raquetteLeft.position += raquetteVelocity;
            }

            if (keyboard.IsKeyDown(Keys.Z) && raquetteRight.position.Y > raquetteRight.image.Height / 2 + 8)
            {
                raquetteRight.position -= raquetteVelocity;
            }
            else if (keyboard.IsKeyDown(Keys.S) && raquetteRight.position.Y < _graphics.PreferredBackBufferHeight - raquetteRight.image.Height / 2 - 8)
            {
                raquetteRight.position += raquetteVelocity;
            }

            balle.position += balle.velocity;
            if (balle.position.Y > _graphics.PreferredBackBufferHeight - balle.image.Height / 2)
            {
                balle.velocity.Y *= -1;
                balle.position.Y = _graphics.PreferredBackBufferHeight - balle.image.Height / 2;
                bangEffect.Play();
            }
            if (balle.position.Y < balle.image.Height / 2)
            {
                balle.velocity.Y *= -1;
                balle.position.Y = balle.image.Height / 2;
                bangEffect.Play();
            }

            Rectangle recBalle = new Rectangle((int)balle.position.X, (int)balle.position.Y, balle.image.Width, balle.image.Height);
            Rectangle recRaquetteLeft = new Rectangle((int)raquetteLeft.position.X, (int)raquetteLeft.position.Y - raquetteLeft.image.Height / 2, raquetteLeft.image.Width, raquetteLeft.image.Height);
            Rectangle recRaquetteRight = new Rectangle((int)raquetteRight.position.X, (int)raquetteRight.position.Y - raquetteRight.image.Height / 2, raquetteRight.image.Width, raquetteRight.image.Height);

            bool collisionRaquetteLeft = recBalle.Intersects(recRaquetteLeft);
            bool collisionRaquetteRight = recBalle.Intersects(recRaquetteRight);

            if (collisionRaquetteLeft)
            {
                balle.position.X = _graphics.PreferredBackBufferWidth - raquetteLeft.image.Width - balle.image.Width;
                balle.velocity.X *= -1;
                pingEffect.Play();
            }
            if (collisionRaquetteRight)
            {
                balle.position.X = raquetteRight.image.Width + balle.image.Width;
                balle.velocity.X *= -1;
                pingEffect.Play();
            }
            if (balle.position.X < 0)
            {
                scoreLeft++;
                Reset();
            }
            if (balle.position.X > _graphics.PreferredBackBufferWidth)
            {
                scoreRight++;
                Reset();
            }

            if (dt >= 15f && dt <= 30f)
            {
                if (balle.velocity.X < 0)
                {
                    if (balle.velocity.Y < 0)
                        balle.velocity.Y = -8;
                    else
                        balle.velocity.Y = 8;

                    balle.velocity.X = -8;
                }
                else if (balle.velocity.X > 0)
                {
                    if (balle.velocity.Y < 0)
                        balle.velocity.Y = -8;
                    else
                        balle.velocity.Y = 8;
                    balle.velocity.X = 8;
                }
            }
            if (dt >= 30f && dt <= 50f)
            {
                if (balle.velocity.X < 0)
                {
                    if (balle.velocity.Y < 0)
                        balle.velocity.Y = -10;
                    else
                        balle.velocity.Y = 10;

                    balle.velocity.X = -10f;
                }
                else if (balle.velocity.X > 0)
                {
                    if (balle.velocity.Y < 0)
                        balle.velocity.Y = -10;
                    else
                        balle.velocity.Y = 10;

                    balle.velocity.X = 10;
                }
            }

            if (scoreLeft >= SCOREMAX || scoreRight >= SCOREMAX)
            {
                MediaPlayer.Stop();
                gameOver = true;
                endGame();
            }
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            SpriteFont spriteFont = Content.Load<SpriteFont>("policeScore");
            SpriteFont spriteFontTime = Content.Load<SpriteFont>("time");
            _spriteBatch.Begin();

            if (!gameOver)
            {

                _spriteBatch.Draw(raquetteLeft.image, raquetteLeft.position, null, Color.White, 0, new Vector2(raquetteLeft.image.Width / 2, raquetteLeft.image.Height / 2), 1, SpriteEffects.None, 0);
                _spriteBatch.Draw(raquetteRight.image, raquetteRight.position, null, Color.White, 0, new Vector2(raquetteRight.image.Width / 2, raquetteRight.image.Height / 2), 1, SpriteEffects.None, 0);
                _spriteBatch.Draw(imageLigne, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), null, Color.White, 0, new Vector2(imageLigne.Width / 2, imageLigne.Height / 2), 1, SpriteEffects.None, 0);
                _spriteBatch.Draw(balle.image, balle.position, null, Color.White, 0, new Vector2(balle.image.Width / 2, balle.image.Height / 2), 1, SpriteEffects.None, 0);


                _spriteBatch.DrawString(spriteFontTime, String.Format("{0:0.0.0}", dt), new Vector2(_graphics.PreferredBackBufferWidth / 4, 25), Color.White);
                _spriteBatch.DrawString(spriteFont, scoreLeft.ToString(), new Vector2(_graphics.PreferredBackBufferWidth / 2 + 50, 50), Color.White);
                _spriteBatch.DrawString(spriteFont, scoreRight.ToString(), new Vector2(_graphics.PreferredBackBufferWidth / 2 - 75, 50), Color.White);

            }
            else
            {
                if (scoreLeft >= SCOREMAX)
                {
                    _spriteBatch.DrawString(spriteFont, "GAME OVER", new Vector2(_graphics.PreferredBackBufferWidth / 2 - spriteFont.Texture.Width / 3, _graphics.PreferredBackBufferHeight / 2 - 200), Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(spriteFont, "LEFT WIN", new Vector2(_graphics.PreferredBackBufferWidth / 2 - spriteFont.Texture.Width / 4 + 50, _graphics.PreferredBackBufferHeight / 2 - 50), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                }
                else if (scoreRight >= SCOREMAX)
                {
                    _spriteBatch.DrawString(spriteFont, "GAME OVER", new Vector2(_graphics.PreferredBackBufferWidth / 2 - spriteFont.Texture.Width / 3, _graphics.PreferredBackBufferHeight / 2 - 200), Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(spriteFont, "RIGHT WIN", new Vector2(_graphics.PreferredBackBufferWidth / 2 - spriteFont.Texture.Width / 4 + 30, _graphics.PreferredBackBufferHeight / 2 - 50), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
