using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

public class Paddle
{
    public Texture2D texture;
    public Vector2 position;
    public Vector2 size;
    public Vector2 screenSize;
    public Vector2 velocity;
    public int padSpeed;
    public int padSpeed2;
    public BoundingBox boundingBox;

    public Paddle(Texture2D t, Vector2 p, Vector2 s, int screenWidth, int screenHeight, BoundingBox b)
    {
        texture = t;
        position = p;
        size = s;
        screenSize = new Vector2(screenWidth, screenHeight);
        boundingBox = b;
        padSpeed = 5;
        padSpeed2 = -5;
    }

    //updates boundingBox in respect to the current paddle's position
    public void updateBoundingBox()
    {
        this.boundingBox.Min.X = this.position.X;
        this.boundingBox.Min.Y = this.position.Y;
        this.boundingBox.Max.X = this.position.X + this.size.X;
        this.boundingBox.Max.Y = this.position.Y + this.size.Y;
    }

    //method for paddle control
    public void Control()
    {
        KeyboardState kbs = Keyboard.GetState();
        if (kbs.IsKeyDown(Keys.Up))
        {
            position += new Vector2(0, padSpeed2);
        }
        if (kbs.IsKeyDown(Keys.Down))
        {
            position += new Vector2(0, padSpeed);
        }
        updateBoundingBox();
    }   

    //method for increasing paddle speed as game progresses
    public void increasePad()
    {
        padSpeed++;
        padSpeed2--;
    }
}
