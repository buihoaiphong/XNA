using System;
using System.Text;
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

public class Game1 : Microsoft.Xna.Framework.Game
{
    NetworkHelper networkHelper;
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    SpriteFont Arial;
    Texture2D splashScreen;
    Texture2D ret;
    public static Camera camera;        // Have a camera usable by the terrain class
    public static Terrain terrain;      // Have a terrain usable by the camera class
    public BasicModel playerModel;
    public BasicModel bulletModel;
    public BasicModel jumpPadModel1;
    public BasicModel gun;

    BoundingSphere p1sphere;
    BoundingSphere p2sphere;
    BoundingSphere jumpPadsphere1;

    public Vector3 Player2 = new Vector3(100, 100, 100);
    public Vector3 hostPos = new Vector3(100, 100, 100);
    public Vector3 guestPos = new Vector3(-100, 100, -100);
    public Vector3 jumpPadLocation1 = new Vector3(0, 140, -250);
    public Vector3 gunPosition;
    public Vector3 pAngle;
    //public Vector3 jumpPadLocation1 = new Vector3(180, 30, 250);

    public List<Vector3> bulletPos = new List<Vector3>();
    public List<Vector3> bulletVel = new List<Vector3>();

    public bool Host = false;
    public bool Guest = false;
    public bool x = false;

    public double shotTimer = 0;

    public float bulletSpeed = 500f;
    public float rot;

    public int p1kills = 0;
    public int p1deaths = 0;
    public int p2kills = 0;
    public int p2deaths = 0;

    KeyboardState oldkeyboard;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Components.Add(new GamerServicesComponent(this));
    }

    protected override void Initialize()
    {
        networkHelper = new NetworkHelper();
        Window.Title = "Doltmuck";
        graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        camera = new Camera(this, Vector3.Zero);
        Components.Add(camera);         // Add camera to components
        terrain = new Terrain(this);    // Make a terrain
        oldkeyboard = Keyboard.GetState();

        gunPosition = camera.cameraPosition;
        gunPosition.Z -= 11;
        gunPosition.X += 6;
        gunPosition.Y -= 2;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        //tertexture = Content.Load<Texture2D>("terraintex");
        spriteBatch = new SpriteBatch(GraphicsDevice);
        terrain.Load("heightMapR", 257, 257, 3f, .5f, Content.Load<Texture2D>("tTexture"));
        terrain.texture = Content.Load<Texture2D>("tTexture");

        Arial = Content.Load<SpriteFont>("Arial");
        splashScreen = Content.Load<Texture2D>("DoltMuck");
        ret = Content.Load<Texture2D>("reticule");
        playerModel = new BasicModel(Content.Load<Model>("spaceship"));
        jumpPadModel1 = new BasicModel(Content.Load<Model>("spaceship"));
        bulletModel = new BasicModel(Content.Load<Model>("spaceship"));
        gun = new BasicModel(Content.Load<Model>("Lasers"));
    }

    protected override void UnloadContent()
    {
        splashScreen.Dispose();
        ret.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        networkHelper.Update();
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))   // If ESC is pressed
            this.Exit();    // Exit the game

        if (!networkHelper.gameStart)
        {
            if (keyboardState.IsKeyDown(Keys.F1))
            {
                networkHelper.SignInGamer();
            }

            if (keyboardState.IsKeyDown(Keys.F2))
            {
                networkHelper.CreateSession();
                Host = true;
                camera = new Camera(this, hostPos);
                Components.Add(camera);         // Add camera to components
                Player2 = new Vector3(100, 100, 100);
                playerModel = new BasicModel(Content.Load<Model>("spaceship"));
            }

            if (keyboardState.IsKeyDown(Keys.F3))
            {
                networkHelper.FindSession();
                Guest = true;
                camera = new Camera(this, guestPos);
                Components.Add(camera);         // Add camera to components
                Player2 = new Vector3(100, 100, 100);
                playerModel = new BasicModel(Content.Load<Model>("spaceship"));
            }

            if (keyboardState.IsKeyDown(Keys.F4))
            {
                networkHelper.AsyncFindSession();
                Guest = true;
                camera = new Camera(this, guestPos);
                Components.Add(camera);         // Add camera to components
                Player2 = new Vector3(100, 100, 100);
                playerModel = new BasicModel(Content.Load<Model>("spaceship"));
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && (gameTime.TotalGameTime.TotalSeconds - shotTimer) >= 1)
            {
                shotTimer = gameTime.TotalGameTime.TotalSeconds;
                bulletPos.Add(camera.cameraPosition);
                bulletVel.Add(camera.forward);
                x = true;
            }
            if (bulletPos.Count > 0)
                bulletModel.Update(bulletPos[0]);
            playerModel.Update(Player2);
            jumpPadModel1.Update(jumpPadLocation1);

            if (camera.angle != pAngle)
            {
                Vector3 newgunPosition = gunPosition - camera.cameraPosition;
                newgunPosition = Vector3.Transform(newgunPosition, Matrix.CreateRotationY(pAngle.Y - camera.angle.Y));
                gunPosition = newgunPosition + camera.cameraPosition;
            }
            if (pAngle.X != camera.angle.X)
            {
                float dotprod, angle, degree;
                Vector3 tempAngle = gunPosition - camera.cameraPosition;
                tempAngle = Vector3.Transform(tempAngle, Matrix.CreateRotationY(0 - camera.angle.Y));
                
                dotprod = Vector3.Dot(camera.angle, tempAngle);
                angle = (float)Math.Acos(dotprod);
                degree = angle * 180 / MathHelper.Pi;
                Console.WriteLine(degree);

                tempAngle = Vector3.Transform(tempAngle, Matrix.CreateRotationX(pAngle.X - camera.angle.X));
                tempAngle = Vector3.Transform(tempAngle, Matrix.CreateRotationY(camera.angle.Y - 0));
                tempAngle = tempAngle + camera.cameraPosition;

            }
            /*gunPosition = camera.cameraPosition;
            gunPosition.Z -= 11;
            gunPosition.X += 6;
            gunPosition.Y -= 2;*/
            gunPosition.Y = camera.cameraPosition.Y - 2;
            //gunPosition.X = camera.cameraPosition.X + 6;
            //gunPosition.Z = camera.cameraPosition.Z - 11;
            gun.Update(gunPosition);

            pAngle = camera.angle;
        }
        else
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && (gameTime.TotalGameTime.TotalSeconds - shotTimer) >= 1)
            {
                shotTimer = gameTime.TotalGameTime.TotalSeconds;
                bulletPos.Add(camera.cameraPosition);
                bulletVel.Add(camera.forward);
                x = true;
            }

            if (bulletPos.Count > 0)
                bulletModel.Update(bulletPos[0]);

            jumpPadModel1.Update(jumpPadLocation1);

            if (networkHelper.p2hit)
            {
                ++p2kills;
                ++p1deaths;
                if (Host)
                {
                    camera.cameraPosition = hostPos;
                }
                else
                {
                    camera.cameraPosition = guestPos;
                }
            }
        }

        if (networkHelper.SessionState == NetworkSessionState.Playing)
        {
            // Send any key pressed to the remote player
            networkHelper.SendMessage(camera.cameraPosition);

            // Recieve the keys from the remote player
            networkHelper.ReceiveMessage();
            Player2 = networkHelper.Player2;
            playerModel.rot = networkHelper.rot;
            playerModel.Update(Player2);
        }
        bulletMove(gameTime);
        BulletCollision();                                        //check collision
        PlayerCollision();
        if (camera.gameStart != networkHelper.gameStart)
            camera.gameStart = networkHelper.gameStart;
        base.Update(gameTime);
    }
    public void PlayerCollision()
    {
        p1sphere = new BoundingSphere(camera.cameraPosition, 30f);
        jumpPadsphere1 = new BoundingSphere(jumpPadLocation1, 5f);
        if (p1sphere.Intersects(jumpPadsphere1))
        {
            camera.jumpPad = true;
            camera.jumpPadinc = true;
            camera.cameraPosition.Y += 5f;
        }
    }
    public void bulletMove(GameTime gametime)
    {
        for (int x = 0; x < bulletPos.Count; x++)
        {
            Vector3 newbulletPos = bulletPos[x];
            Vector3 newbulletVel = bulletVel[x];
            newbulletVel.Normalize();
            newbulletPos -= newbulletVel * bulletSpeed * (float)gametime.ElapsedGameTime.TotalSeconds;
            bulletPos[x] = newbulletPos;
        }
    }

    public void BulletCollision()
    {
        p1sphere = new BoundingSphere(camera.cameraPosition, 10f);
        p2sphere = new BoundingSphere(Player2, 10f);
        for (int x = 0; x < bulletPos.Count; x++)
        {
            BoundingSphere bulletSphere = new BoundingSphere(bulletPos[x], .5f);
            if (bulletSphere.Intersects(p2sphere))
            {
                networkHelper.p1hit = true;
                bulletPos.RemoveAt(x);
                bulletVel.RemoveAt(x);
                x = 0;
                ++p1kills;
                ++p1deaths;
                playerModel.rot += 90;
            }
        }
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        //terrain.Draw(gameTime);     // Draw the terrain
        if (!networkHelper.gameStart)
        {
            spriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            spriteBatch.Draw(splashScreen, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Arial, "F1 to sign in", new Vector2(500, 450), Color.Yellow);
            spriteBatch.DrawString(Arial, "F2 to create a session", new Vector2(500, 470), Color.Yellow);
            spriteBatch.DrawString(Arial, "F3 to find session", new Vector2(500, 490), Color.Yellow);
            spriteBatch.DrawString(Arial, networkHelper.Message, new Vector2(300, 570), Color.Yellow);
            spriteBatch.End();

            playerModel.Draw(Player2, false);
            jumpPadModel1.Draw(jumpPadLocation1, false);
            if (bulletPos.Count > 0)
                bulletModel.Draw(bulletPos[0], false);

            gun.Draw(gunPosition, true);
        }

        if (networkHelper.gameStart)
        {
            terrain.Draw(gameTime);     // Draw the terrain
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            spriteBatch.DrawString(Arial, p1kills + " VS " + p2kills, new Vector2((graphics.PreferredBackBufferWidth / 2) - 20, 10), Color.White);
            spriteBatch.Draw(ret, new Vector2(400, 300), Color.White);
            spriteBatch.End();
            playerModel.Draw(Player2, false);
            jumpPadModel1.Draw(jumpPadLocation1, false);
            for (int x = 0; x < bulletPos.Count; ++x)
            {
                bulletModel.Draw(bulletPos[x], false);
            }

        }
        base.Draw(gameTime);
    }
}

