using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Project3
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //font
        SpriteFont font;

        //keyboard state var
        KeyboardState pkbs;

        //sprites
        Sprite background;
        Sprite ball;

        //paddle
        Paddle pad;

        //song
        Song bTrack;

        //xap audio stuff
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        //animation speed variables
        int timeSinceLastFrame = 0;
        int msPerFrame = 50;

        //sprite sheet frame variables
        Point fSize = new Point(21, 20);
        Point cFrame = new Point(0, 0);
        Point sSize = new Point(2, 2);

        //misc placeholders
        int p = 0;
        int scorep = 0;
        int scorec = 0;
        int ballSpeed = 5;
        int angle = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //background load
            background = new Sprite(Content.Load<Texture2D>("pac_back"), new Vector2(150, 60), new Vector2(0, 0), 800, 600);

            //pad load
            pad = new Paddle(Content.Load<Texture2D>("pad"), new Vector2(50, 250), new Vector2(13, 50), 800, 600, new BoundingBox(new Vector3(0,0,0), new Vector3(13,50,0)));

            //ball load/set
            ball = new Sprite(Content.Load<Texture2D>("ball"), new Vector2(600, 250), new Vector2(20, 21), 800, 600, new BoundingSphere(new Vector3(0, 0, 0), 0));
            ball.velocity = new Vector2(ballSpeed, 1);

            //xap loads
            audioEngine = new AudioEngine(@"Content\pong.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content \Sound Bank.xsb");

            //track load/play
            bTrack = Content.Load<Song>("rap_chop");
            MediaPlayer.Play(bTrack);

            //font load
            font = Content.Load<SpriteFont>("SpriteFont1");
        }

        protected override void UnloadContent()
        {
            background.texture.Dispose();
            pad.texture.Dispose();
            ball.texture.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            //give player pad control
            pad.Control();

            //ball move/check
            if (ball.Move() == 1)
            {
                soundBank.PlayCue("kick_shell");
            }

            //checks if the wall behind the paddle was hit, and increases computer's score
            if (ball.wallCheck() == 1)
            {
                defaultLoads();
                scorec++;
            }

            //checks for music pause
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.P) && pkbs.IsKeyDown(Keys.P) == false)
            {
                if (p == 1)
                {
                    MediaPlayer.Resume();
                    p = 0;
                }
                else
                {
                    MediaPlayer.Pause();
                    p = 1;
                }
            }
            pkbs = kbs;

            //bounding sphere checks for paddle and ball
            if (ball.boundingSphere.Intersects(pad.boundingBox))
            {
                ball.velocity = new Vector2(ballSpeed++, angle++);
                soundBank.PlayCue("bonk");
                scorep++;
                pad.increasePad();
            }

            //game over check
            if (scorep == 25 || scorec == 25)
            {
                scorep = scorec = 0;
                soundBank.PlayCue("victory");
                defaultLoads();
            }

            //animate speed
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > msPerFrame)
            {
                timeSinceLastFrame -= msPerFrame;
                ++cFrame.X;
                if (cFrame.X >= sSize.X)
                {
                    cFrame.X = 0;
                    ++cFrame.Y;
                    if (cFrame.Y >= sSize.Y)
                    {
                        cFrame.Y = 0;
                    }
                }
            }
            
            base.Update(gameTime);
        }

        //draws scoreboard to top of screen
        private void DrawText()
        {
            spriteBatch.DrawString(font, scorep + ":" + scorec, new Vector2(385, 20), Color.White);
        }

        //reloads default speeds for ball/pad alike
        private void defaultLoads()
        {
            pad.padSpeed = 5;
            pad.padSpeed2 = -5;
            ballSpeed = 5;
            angle = 1;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(background.texture, background.position, Color.White);
            spriteBatch.Draw(pad.texture, pad.position, Color.White);
            spriteBatch.Draw(ball.texture, ball.position, new Rectangle(cFrame.X * fSize.X, cFrame.Y * fSize.Y, 21, 20), Color.White);
            DrawText();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
