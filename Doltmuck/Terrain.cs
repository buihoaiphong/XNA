using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;

public class Terrain : DrawableGameComponent
{
    VertexBuffer vb;
    IndexBuffer ib;
    BasicEffect effect;
    public Texture2D texture;

    public int vertexCountX;
    public int vertexCountZ;
    public float blockScale;
    public float heightScale;
    public byte[] heightmap;
    public int numVertices;
    public int numTriangles;

    public Vector2 StartPosition;

    public Terrain(Game game)
        : base(game)
    {

    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public void Load(string heightmapFileName, int vertexCountX,
                        int vertexCountZ, float blockScale, float heightScale, Texture2D texture)
    {
        Initialize();

        effect = new BasicEffect(GraphicsDevice, null);
        vertexCountX = 257;
        vertexCountZ = 257;
        blockScale = 3f;
        heightScale = .5f;
        FileStream filestream = File.OpenRead(Game.Content.RootDirectory + "/" + heightmapFileName + ".raw");
        int heightmapSize = vertexCountX * vertexCountZ;
        heightmap = new byte[heightmapSize];
        filestream.Read(heightmap, 0, heightmapSize);
        //Be sure to close the stream
        filestream.Close();
        GenerateTerrainMesh();

    }

    private void GenerateTerrainMesh()
    {
        vertexCountX = 257;
        vertexCountZ = 257;
        blockScale = 3f;
        heightScale = .5f;
        numVertices = vertexCountX * vertexCountZ;
        numTriangles = (vertexCountX - 1) * (vertexCountZ - 1) * 2;
        int[] indices = GenerateTerrainIndices();
        VertexPositionTexture[] vertices = GenerateTerrainVertices(indices);
        vb = new VertexBuffer(GraphicsDevice, numVertices * VertexPositionTexture.SizeInBytes,
                                BufferUsage.WriteOnly);
        vb.SetData<VertexPositionTexture>(vertices);
        ib = new IndexBuffer(GraphicsDevice, numTriangles * 3 * sizeof(int),
                                BufferUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
        ib.SetData<int>(indices);

    }

    private int[] GenerateTerrainIndices()
    {
        vertexCountX = 257;
        vertexCountZ = 257;
        blockScale = 3f;
        heightScale = .5f;
        int numIndices = numTriangles * 3;
        int[] indices = new int[numIndices];
        int indicesCount = 0;
        for (int i = 0; i < (vertexCountZ - 1); i++) //All Rows except last
            for (int j = 0; j < (vertexCountX - 1); j++) //All Columns except last
            {
                int index = j + i * vertexCountZ; //2D coordinates to linear
                //First Triangle Vertices
                indices[indicesCount++] = index;
                indices[indicesCount++] = index + 1;
                indices[indicesCount++] = index + vertexCountX + 1;

                //Second Triangle Vertices
                indices[indicesCount++] = index + vertexCountX + 1;
                indices[indicesCount++] = index + vertexCountX;
                indices[indicesCount++] = index;
            }
        return indices;
    }

    private VertexPositionTexture[] GenerateTerrainVertices(int[] terrainIndeces)
    {
        vertexCountX = 257;
        vertexCountZ = 257;
        blockScale = 3f;
        heightScale = .5f;
        float halfTerrainWidth = (vertexCountX - 1) * blockScale * .5f;
        float halfTerrainDepth = (vertexCountZ - 1) * blockScale * .5f;
        float tuDerivative = 1.0f / (vertexCountX - 1);
        float tvDerivative = 1.0f / (vertexCountZ - 1);
        StartPosition = new Vector2(-halfTerrainWidth, -halfTerrainDepth);

        VertexPositionTexture[] vertices = new VertexPositionTexture[vertexCountX * vertexCountZ];
        int vertexCount = 0;
        float tu = 0;
        float tv = 0;
        for (float i = -halfTerrainDepth; i <= halfTerrainDepth; i += blockScale)
        {
            tu = 0.0f;
            for (float j = -halfTerrainWidth; j <= halfTerrainWidth; j += blockScale)
            {
                vertices[vertexCount].Position = new Vector3(j, heightmap[vertexCount] * heightScale, i);
                vertices[vertexCount].TextureCoordinate = new Vector2(tu, tv);

                tu += tuDerivative;
                vertexCount++;
            }
            tv += tvDerivative;
        }

        return vertices;

    }

    public float getHeight(float positionX, float positionZ)
    {
        float height = -999999.0f;
        if (heightmap == null)
            return height;

        Vector2 positionInGrid = new Vector2(positionX - StartPosition.X, positionZ - StartPosition.Y);

        Vector2 blockPosition = new Vector2(positionInGrid.X / blockScale, positionInGrid.Y / blockScale);

        if (blockPosition.X >= 0 && blockPosition.X < (vertexCountX - 1) &&
            blockPosition.Y >= 0 && blockPosition.Y < (vertexCountZ - 1))
        {
            //Remainder of calculations
            Vector2 blockOffset = new Vector2(blockPosition.X - (int)blockPosition.X, blockPosition.Y - (int)blockPosition.Y);
            int vertexIndex = (int)blockPosition.X + (int)blockPosition.Y * vertexCountX;
            float height1 = heightmap[vertexIndex + 1];
            float height2 = heightmap[vertexIndex];
            float height3 = heightmap[vertexIndex + vertexCountX + 1];
            float height4 = heightmap[vertexIndex + vertexCountX];

            float heightIncX, heightIncY;

            //Top triangle
            if (blockOffset.X > blockOffset.Y)
            {
                heightIncX = height1 - height2;
                heightIncY = height3 - height1;
            }
            //Bottom triangle
            else
            {
                heightIncX = height3 - height4;
                heightIncY = height4 - height2;
            }
            // Linear interpolation to find the height inside the triangle
            float lerpHeight = height2 + heightIncX * blockOffset.X + heightIncY * blockOffset.Y;
            height = lerpHeight * heightScale;
        }
        return height;
    }

    public float getHeight(Vector2 position)
    {
        return getHeight(position.X, position.Y);
    }

    public float getHeight(Vector3 position)
    {
        return getHeight(position.X, position.Z);
    }

    public float? Intersects(Ray ray)
    {
        //This won't be changed if the Ray doesn't collide with terrain
        float? collisionDistance = null;
        //Size of step is half of blockScale
        Vector3 rayStep = ray.Direction * blockScale * 0.5f;
        //Need to save start position to find total distance once collision point is found
        Vector3 rayStartPosition = ray.Position;

        Vector3 lastRayPosition = ray.Position;
        ray.Position += rayStep;
        float height = getHeight(ray.Position);

        while (ray.Position.Y > height && height >= 0)
        {

            lastRayPosition = ray.Position;
            ray.Position += rayStep;
            height = getHeight(ray.Position);
        }

        if (height >= 0) //Lowest possible point of terrain
        {
            Vector3 startPosition = lastRayPosition;
            Vector3 endPosition = ray.Position;
            // Binary search. Find the exact collision point
            for (int i = 0; i < 32; i++)
            {
                // Binary search pass
                Vector3 middlePoint = (startPosition + endPosition) * 0.5f;
                if (middlePoint.Y < height)
                    endPosition = middlePoint;
                else
                    startPosition = middlePoint;
            }
            Vector3 collisionPoint = (startPosition + endPosition) * 0.5f;
            collisionDistance = Vector3.Distance(rayStartPosition,
            collisionPoint);
        }//end if
        return collisionDistance;
    }

    public override void Draw(GameTime gameTime)
    {
        //BasicEffect effect = new BasicEffect(GraphicsDevice, null);
        effect.World = Matrix.Identity; //No transformation of the terrain
        effect.View = Game1.camera.view;
        effect.Projection = Game1.camera.project;
        effect.Texture = texture;
        effect.TextureEnabled = true;
        GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexPositionTexture.SizeInBytes); //Set vertices
        GraphicsDevice.Indices = ib; //Set indices
        GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements); //Decalre type of vertices
        effect.Begin(); //Begin effect
        foreach (EffectPass CurrentPass in effect.CurrentTechnique.Passes)
        {
            CurrentPass.Begin();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles); //Draw all triangles that make up the mesh
            CurrentPass.End();
        }
        effect.End();
        base.Draw(gameTime);

    }
}

