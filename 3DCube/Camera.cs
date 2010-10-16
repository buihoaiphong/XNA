using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//xna includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Camera : Microsoft.Xna.Framework.GameComponent
{
    public Matrix view { get; protected set; }
    public Matrix project { get; protected set; }

    public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
        : base(game)
    {
        view = Matrix.CreateLookAt(pos, target, up);
        project = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height, 1, 100);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}