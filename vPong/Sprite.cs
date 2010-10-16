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

public class Sprite
{
    public Texture2D texture;
    public Vector2 position;
    public Vector2 size;
    public Vector2 screenSize;
    public Vector2 velocity;
    public BoundingSphere boundingSphere;

    public Sprite(Texture2D t, Vector2 p, Vector2 s, int screenWidth, int screenHeight)
    {
        texture = t;
        position = p;
        size = s;
        screenSize = new Vector2(screenWidth, screenHeight);
    }

    public Sprite(Texture2D t, Vector2 p, Vector2 s, int screenWidth, int screenHeight, BoundingSphere b)
    {
        texture = t;
        position = p;
        size = s;
        screenSize = new Vector2(screenWidth, screenHeight);
        boundingSphere = b;
    }

    public void updateBoundingSphere()
    {
        this.boundingSphere.Center.X = this.position.X;
        this.boundingSphere.Center.Y = this.position.Y;
    }

    //moves sprite, returns hit
    public int Move()
    {
        int hit = 0;
        if (position.X + size.X + velocity.X > screenSize.X) // bottom
        {
            velocity.X = -velocity.X;
            hit = 1;
        }
        if (position.Y + size.Y + velocity.Y > screenSize.Y) // right
        {
            velocity.Y = -velocity.Y;
            hit = 1;
        }
        if (position.X + velocity.X < 0) // top
        {
            velocity.X = -velocity.X;
            hit = 1;
        }
        if (position.Y + velocity.Y < 0) // left
        {
            velocity.Y = -velocity.Y;
            hit = 1;
        }

        position += velocity;
        updateBoundingSphere();

        return hit;
    }

    //method for ball to check if wall hit, then resets position and velocity
    public int wallCheck()
    {
        int check = 0;
        if (position.X + velocity.X < 0)
        {
            position = new Vector2(300, 200);
            velocity = new Vector2(5, 0);
            check = 1;
        }
        return check;
    }
}
