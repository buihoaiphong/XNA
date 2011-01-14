using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//xna includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Camera : Microsoft.Xna.Framework.DrawableGameComponent
{
    public Matrix view { get; protected set; }
    public Matrix project { get; protected set; }
    public Vector3 cameraPosition;
    public Vector3 cameraDirection;
    NetworkHelper networkHelper;
    KeyboardState oldkeyboard;

    public Ray rayfall;                                        // Ray that tracks grabity
    public Ray raymove;                                        // Ray that tracks collision

    public Vector3 angle = new Vector3();
    public Vector3 forward = new Vector3();
    public Vector3 jumpPadForward = new Vector3();
    public Vector3 jumpPadDirection1 = new Vector3(.99f, 0f, -.043f);
    public Vector3 fallingvel = new Vector3(0, -.03f, 0); // Gravity
    public Vector3 maxfallvel = new Vector3(0, -1.5f, 0);

    public float speed = 50f;
    public float speed2 = 50f;                          // How fast the camera moves
    public float jumpPadSpeed = 200f;
    public float totalYaw = MathHelper.PiOver4 / 2;     // Allows 45°
    public float currentYaw = 0;                        // Keeps track of the Yaw rotation
    public float totalPitch = MathHelper.PiOver4 / 2;   // Allows 45
    public float currentPitch = 0;                      // Keeps track of the Pitch rotaion
    public float height;                                // The height of the terrain
    public float heightmax = .45f;                      // Maximum height difference that can be climed
    public float jump = 0f;                              // How much to increase camera .Y position when jumping
    public float delta;
    public float jumpPading = 0f;

    public double timeSinceLastJump = 0;

    public bool gameStart = false;
    public bool jumpPad = false;
    public bool jumpPadinc = false;
    public bool jumping = false;                        // Jumping variable 

    public Camera(Game game, Vector3 pos)
        : base(game)
    {
        project = Matrix.Identity;
        view = Matrix.Identity;
        cameraPosition = new Vector3(0, 400, 0);//pos; 
    }

    public override void Initialize()
    {
        networkHelper = new NetworkHelper();
        int centerX = Game.Window.ClientBounds.Width / 2;
        int centerY = Game.Window.ClientBounds.Width / 2;

        Mouse.SetPosition(centerX, centerY);
        oldkeyboard = Keyboard.GetState();

        rayfall.Position = cameraPosition;              // When the camera is made set rayfall position at the same spot
        rayfall.Direction = Vector3.Down;               //              ||         set rayfall direction at the same direction   

        base.Initialize();
    }

    protected override void LoadGraphicsContent(bool loadAllContent)
    {
        float ratio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
        project = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ratio, 10, 10000);
        base.LoadGraphicsContent(loadAllContent);
    }

    public override void Update(GameTime gameTime)
    {
        networkHelper.Update();
        if (gameStart)
        {
            Console.WriteLine(speed);
            raymove.Position = cameraPosition;              // Set the ray positions to the camera position
            rayfall.Position = cameraPosition;             
            if (jumping)
            {
                cameraPosition.Y += jump;
                jump -= .08f;
                if (jump <= -2f)
                {
                    jumping = false;
                }
            }
            if (jumpPad)
            {
                cameraPosition -= jumpPadDirection1 * jumpPadSpeed * delta;
                cameraPosition.Y -= jumpPading;
                if (jumpPading >= -5f && jumpPadinc)
                {
                    jumpPading -= .08f;
                }
                else
                {
                    jumpPadinc = false;
                    jumpPading = 0f;
                }
            }

            if (Game1.terrain.Intersects(rayfall) > 20)
            {
                // If the ray intersect distance is greater than 20...
                cameraPosition += fallingvel;   // Move the camera down
                if (fallingvel.Y > maxfallvel.Y)
                    fallingvel *= 1.1f;            // Increase falling speed
            }

            else
            {
                // If the ray intersect distance is less than 20...
                fallingvel = new Vector3(0, -.03f, 0);      // Reset gravity
                jumping = false;                            // No longer jumping
                jumpPad = false;
                heightmax = .45f;                           // Reset maximum height climbable
                jumpPading = 0f;
            }

            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouse = Mouse.GetState();
            KeyboardState kbs = Keyboard.GetState();

            int centerX = Game.Window.ClientBounds.Width / 2;
            int centerY = Game.Window.ClientBounds.Width / 2;

            Mouse.SetPosition(centerX, centerY);

            angle.X += MathHelper.ToRadians((mouse.Y - centerY) * speed2 * 0.01f);
            angle.Y += MathHelper.ToRadians((mouse.X - centerX) * speed2 * 0.01f);

            forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(angle.X), (float)Math.Cos(-angle.Y)));
            Vector3 left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));

            if (!jumpPad)
            {
                Console.WriteLine(cameraPosition);
                if (kbs.IsKeyDown(Keys.W))
                {
                    // When W is pressed go forward
                    height = Game1.terrain.getHeight(cameraPosition);
                    Vector3 newcameraPosition = cameraPosition;
                    Vector3 newcameraDirection = forward;
                    newcameraDirection.Y = 0;
                    newcameraPosition -= newcameraDirection * speed * delta;
                    float newheight = Game1.terrain.getHeight(newcameraPosition);
                    height = newheight - height;

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && oldkeyboard.IsKeyDown(Keys.LeftShift))
                    {
                        cameraPosition -= newcameraDirection * (speed * 1.7f) * delta;
                    }
                    else
                    {
                        cameraPosition -= newcameraDirection * speed * delta;
                    }
                    if (!jumping && height > 0)
                    {
                        cameraPosition.Y += height;
                    }
                }
                if (kbs.IsKeyDown(Keys.S))
                {
                    // When S is pressed go forward
                    height = Game1.terrain.getHeight(cameraPosition);
                    Vector3 newcameraPosition = cameraPosition;
                    Vector3 newcameraDirection = forward;
                    newcameraDirection.Y = 0;
                    newcameraPosition += newcameraDirection * speed * delta;
                    float newheight = Game1.terrain.getHeight(newcameraPosition);
                    height = newheight - height;

                    cameraPosition += newcameraDirection * speed * delta;
                    if (!jumping)
                    {
                        cameraPosition.Y += height;
                    }
                }
                if (kbs.IsKeyDown(Keys.A))
                {
                    // When A is pressed go forward
                    height = Game1.terrain.getHeight(cameraPosition);
                    Vector3 newcameraPosition = cameraPosition;
                    Vector3 newcameraDirection = left;
                    newcameraDirection.Y = 0;
                    newcameraPosition -= newcameraDirection * speed * delta;
                    float newheight = Game1.terrain.getHeight(newcameraPosition);
                    height = newheight - height;

                    cameraPosition -= left * speed * delta;
                    if (!jumping)
                    {
                        cameraPosition.Y += height;
                    }
                }
                if (kbs.IsKeyDown(Keys.D))
                {
                    // When D is pressed go forward
                    height = Game1.terrain.getHeight(cameraPosition);
                    Vector3 newcameraPosition = cameraPosition;
                    Vector3 newcameraDirection = left;
                    newcameraDirection.Y = 0;
                    newcameraPosition += newcameraDirection * speed * delta;
                    float newheight = Game1.terrain.getHeight(newcameraPosition);
                    height = newheight - height;
         
                    cameraPosition += newcameraDirection * speed * delta;
                    if (!jumping)
                    {
                        cameraPosition.Y += height;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    // If Space Bar is pressed...
                    timeSinceLastJump = gameTime.TotalGameTime.TotalSeconds;
                    speed *= 1.01f;
                    if (speed >= 130)       //cap speed at 130
                    {
                        speed = 130;
                    }
                    if (!jumping)
                    {
                        // If not already jumping...
                        jump = 2f;        // Increase jumping variable
                        heightmax *= 2;       // Increase the maximum height climeable
                        jumping = true;     // Set it to now jumping
                    }
                }
                if (!jumping && (gameTime.TotalGameTime.TotalSeconds - timeSinceLastJump) >= 1)
                {
                    //cameraPosition.Y += height;
                    speed = 50f;
                    timeSinceLastJump = 0;
                }
            }

            view = Matrix.Identity;
            view *= Matrix.CreateTranslation(-cameraPosition);
            view *= Matrix.CreateRotationZ(angle.Z);
            view *= Matrix.CreateRotationY(angle.Y);
            view *= Matrix.CreateRotationX(angle.X);

            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Up, (-MathHelper.PiOver4 / 150)
                        * (mouse.X - centerX)));
            cameraDirection = Vector3.Transform(cameraDirection,                    // Update camera Direction
                                                Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, cameraDirection),
                                                (MathHelper.PiOver4 / 150) * (Mouse.GetState().Y - centerY)));

            cameraDirection.Normalize();
            oldkeyboard = Keyboard.GetState();

        }

        base.Update(gameTime);
    }
}
