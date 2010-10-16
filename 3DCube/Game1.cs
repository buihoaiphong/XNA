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

namespace Project4
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //camera
        Camera camera;
        
        //world vars
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        //angle var
        float angle = 0;

        //cube stuff
        Cube cube;
        BasicEffect effect;

        //cube texture array
        Texture2D[] tex = new Texture2D[6];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //camera init
            camera = new Camera(this, new Vector3(0, 0, 25), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            //cube init
            cube = new Cube(new Vector3(2, 2, 2), new Vector3(0, 0, 0));
            cube.buildCube();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //texture loads
            tex[0] = Content.Load<Texture2D>("pik");
            tex[1] = Content.Load<Texture2D>("trollface");
            tex[2] = Content.Load<Texture2D>("fuuuuuu");
            tex[3] = Content.Load<Texture2D>("toast");
            tex[4] = Content.Load<Texture2D>("slow");
            tex[5] = Content.Load<Texture2D>("bear");
        }

        protected override void UnloadContent()
        {
            //dispose the array of textures
            for (int i = 0; i < 6; ++i)
            {
                tex[i].Dispose();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //keyboard movement - left/right, speed up/slow down spin
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left))
            {
                worldTranslation *= Matrix.CreateTranslation(-.03f, 0, 0);
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                worldTranslation *= Matrix.CreateTranslation(.03f, 0, 0);
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                angle += .01f;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                angle -= .01f;
            }
            
            //angles the cube for auto spin
            worldRotation *= Matrix.CreateFromYawPitchRoll(MathHelper.PiOver4 / 60, angle, 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            effect = new BasicEffect(GraphicsDevice, null);

            effect.World = worldRotation * worldTranslation;
            effect.View = camera.view;
            effect.Projection = camera.project;
            effect.Texture = tex[0];
            effect.TextureEnabled = true;

            //loads a side, changes texture, loads next side, repeat
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.ffront, 0, 2);
                pass.End();

                effect.Texture = tex[1];
                effect.TextureEnabled = true;
                pass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.bback, 0, 2);
                pass.End();

                effect.Texture = tex[2];
                effect.TextureEnabled = true;
                pass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.lleft, 0, 2);
                pass.End();

                effect.Texture = tex[3];
                effect.TextureEnabled = true;
                pass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.rright, 0, 2);
                pass.End();

                effect.Texture = tex[4];
                effect.TextureEnabled = true;
                pass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.ttop, 0, 2);
                pass.End();

                effect.Texture = tex[5];
                effect.TextureEnabled = true;
                pass.Begin();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.bbot, 0, 2);
                pass.End();
            }
            effect.End();
            base.Draw(gameTime);
        }
    }
}
