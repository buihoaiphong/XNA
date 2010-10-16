using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Cube
{
    //size and position of cube
    public Vector3 size;
    public Vector3 position;

    //arrays of vert sides
    public VertexPositionNormalTexture[] ffront;
    public VertexPositionNormalTexture[] bback;
    public VertexPositionNormalTexture[] lleft;
    public VertexPositionNormalTexture[] rright;
    public VertexPositionNormalTexture[] ttop;
    public VertexPositionNormalTexture[] bbot;

    //number of triangles needed, in our case 12 ( 6 * 2 )
    public int triangles = 12;

    public Cube(Vector3 size, Vector3 position)
    {
        this.size = size;
        this.position = position;
    }

    public void buildCube()
    {
         //vertex arrays for each side of the cube
         ffront = new VertexPositionNormalTexture[6];
         bback = new VertexPositionNormalTexture[6];
         lleft = new VertexPositionNormalTexture[6];
         rright = new VertexPositionNormalTexture[6];
         ttop = new VertexPositionNormalTexture[6];
         bbot = new VertexPositionNormalTexture[6];

        //has to be an easier way to do this.  this stuff sets the points of the cube 
        //in relation to its size.  tedious figuring.  
        Vector3 topLeftFront = position + new Vector3(-1.0f, 1.0f, -1.0f) * size;
        Vector3 topRightFront = position + new Vector3(1.0f, 1.0f, -1.0f) * size;
        Vector3 topLeftBack = position + new Vector3(-1.0f, 1.0f, 1.0f) * size;
        Vector3 topRightBack = position + new Vector3(1.0f, 1.0f, 1.0f) * size;

        Vector3 botLeftFront = position + new Vector3(-1.0f, -1.0f, -1.0f) * size;       
        Vector3 botRightFront = position + new Vector3(1.0f, -1.0f, -1.0f) * size;
        Vector3 botLeftBack = position + new Vector3(-1.0f, -1.0f, 1.0f) * size;
        Vector3 botRightBack = position + new Vector3(1.0f, -1.0f, 1.0f) * size;
        
        //this is the texturing coords
        Vector2 tTopLeft = new Vector2(0.5f * size.X, 0.0f * size.Y);
        Vector2 tTopRight = new Vector2(0.0f * size.X, 0.0f * size.Y);
        Vector2 tBotLeft = new Vector2(0.5f * size.X, 0.5f * size.Y);
        Vector2 tBotRight = new Vector2(0.0f * size.X, 0.5f * size.Y);

        //this stuff is for scaling textures in respect to size
        Vector3 front = new Vector3(0.0f, 0.0f, 1.0f) * size;
        Vector3 back = new Vector3(0.0f, 0.0f, -1.0f) * size;
        Vector3 left = new Vector3(-1.0f, 0.0f, 0.0f) * size;
        Vector3 right = new Vector3(1.0f, 0.0f, 0.0f) * size;
        Vector3 top = new Vector3(0.0f, 1.0f, 0.0f) * size;
        Vector3 bot = new Vector3(0.0f, -1.0f, 0.0f) * size;

        //tedius drawing of the 36 verticies needed to complete the cube.  I WANT TO IMPORT MODELS AT THIS POINT.
        
        //begin drawing

        //front verts with position and texture stuff
        ffront[0] = new VertexPositionNormalTexture( topLeftFront, front, tTopLeft );
        ffront[1] = new VertexPositionNormalTexture(botLeftFront, front, tBotLeft);
        ffront[2] = new VertexPositionNormalTexture(topRightFront, front, tTopRight);
        ffront[3] = new VertexPositionNormalTexture(botLeftFront, front, tBotLeft);
        ffront[4] = new VertexPositionNormalTexture(botRightFront, front, tBotRight);
        ffront[5] = new VertexPositionNormalTexture(topRightFront, front, tTopRight);

        //back verts with position and texture stuff
        bback[0] = new VertexPositionNormalTexture( topLeftBack, back, tTopRight );
        bback[1] = new VertexPositionNormalTexture(topRightBack, back, tTopLeft);
        bback[2] = new VertexPositionNormalTexture(botLeftBack, back, tBotRight);
        bback[3] = new VertexPositionNormalTexture(botLeftBack, back, tBotRight);
        bback[4] = new VertexPositionNormalTexture(topRightBack, back, tTopLeft);
        bback[5] = new VertexPositionNormalTexture(botRightBack, back, tBotLeft);

        //top side verts with position/texture stuff
        ttop[0] = new VertexPositionNormalTexture( topLeftFront, top, tBotLeft );
        ttop[1] = new VertexPositionNormalTexture(topRightBack, top, tTopRight);
        ttop[2] = new VertexPositionNormalTexture(topLeftBack, top, tTopLeft);
        ttop[3] = new VertexPositionNormalTexture(topLeftFront, top, tBotLeft);
        ttop[4] = new VertexPositionNormalTexture(topRightFront, top, tBotRight);
        ttop[5] = new VertexPositionNormalTexture(topRightBack, top, tTopRight);

        //bottom side verts with position/texture stuff
        bbot[0] = new VertexPositionNormalTexture( botLeftFront, bot, tTopLeft );
        bbot[1] = new VertexPositionNormalTexture(botLeftBack, bot, tBotLeft);
        bbot[2] = new VertexPositionNormalTexture(botRightBack, bot, tBotRight);
        bbot[3] = new VertexPositionNormalTexture(botLeftFront, bot, tTopLeft);
        bbot[4] = new VertexPositionNormalTexture(botRightBack, bot, tBotRight);
        bbot[5] = new VertexPositionNormalTexture(botRightFront, bot, tTopRight);

        //left side verts with position/texture stuff
        lleft[0] = new VertexPositionNormalTexture( topLeftFront, left, tTopRight );
        lleft[1] = new VertexPositionNormalTexture(botLeftBack, left, tBotLeft);
        lleft[2] = new VertexPositionNormalTexture(botLeftFront, left, tBotRight);
        lleft[3] = new VertexPositionNormalTexture(topLeftBack, left, tTopLeft);
        lleft[4] = new VertexPositionNormalTexture(botLeftBack, left, tBotLeft);
        lleft[5] = new VertexPositionNormalTexture(topLeftFront, left, tTopRight);

        //right side verts with position/texture stuff
        rright[0] = new VertexPositionNormalTexture( topRightFront, right, tTopLeft );
        rright[1] = new VertexPositionNormalTexture(botRightFront, right, tBotLeft);
        rright[2] = new VertexPositionNormalTexture(botRightBack, right, tBotRight);
        rright[3] = new VertexPositionNormalTexture(topRightBack, right, tTopRight);
        rright[4] = new VertexPositionNormalTexture(topRightFront, right, tTopLeft);
        rright[5] = new VertexPositionNormalTexture(botRightBack, right, tBotRight);
    }
}