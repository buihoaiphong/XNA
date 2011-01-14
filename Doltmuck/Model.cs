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

public class BasicModel
{
    public Model model { get; protected set; }
    protected Matrix world = Matrix.Identity;
    protected Matrix worldTranslation = Matrix.Identity;
    public Matrix rotation = Matrix.Identity;
    public float rot = 0;

    public BasicModel(Model m)
    {
        model = LoadModelBS(m);
    }
    //util: transforms a sphere onto the model
    private static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
    {
        Vector3 trans;
        Vector3 scaling;
        Quaternion rot;
        transformationMatrix.Decompose(out scaling, out rot, out trans);

        float maxScale = scaling.X;
        if (maxScale < scaling.Y)
            maxScale = scaling.Y;
        if (maxScale < scaling.Z)
            maxScale = scaling.Z;

        float transformedSphereRadius = originalBoundingSphere.Radius * maxScale;
        Vector3 transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

        BoundingSphere transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

        return transformedBoundingSphere;
    }

    //util: loads the model with the transformed sphere
    private Model LoadModelBS(Model m)
    {
        Model newModel = m;
        Matrix[] ttransform = new Matrix[m.Bones.Count];
        m.CopyAbsoluteBoneTransformsTo(ttransform);

        BoundingSphere cBS = new BoundingSphere();
        foreach (ModelMesh mesh in newModel.Meshes)
        {
            BoundingSphere origMeshSphere = mesh.BoundingSphere;
            BoundingSphere transMeshSphere = TransformBoundingSphere(origMeshSphere, ttransform[mesh.ParentBone.Index]);
            cBS = BoundingSphere.CreateMerged(cBS, transMeshSphere);
        }
        newModel.Tag = cBS;
        return newModel;
    }

    public virtual void Update(Vector3 position)
    {
        worldTranslation = Matrix.CreateTranslation(position);
    }
    public void Draw(Vector3 player, bool isGun)
    {
        //Set transforms
        Matrix[] transforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(transforms);
        //Loop through meshes and their effects
        //rot += .005f;
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect be in mesh.Effects)
            {
                //Set BasicEffect information
                be.EnableDefaultLighting();
                be.Projection = Game1.camera.project;
                be.View = Game1.camera.view;
                if (!isGun)
                    be.World = Matrix.CreateRotationY(-rot + 135f) * worldTranslation * mesh.ParentBone.Transform;  // Update the rotation with the rotation from Game1
                else
                    be.World = Matrix.CreateRotationY(-Game1.camera.angle.Y) * worldTranslation * mesh.ParentBone.Transform;
            }
            //Draw
            mesh.Draw();
        }
    }
    public virtual Matrix GetWorld()
    {
        return world;
        //return world * Matrix.CreateRotationY(3*rot);
    }
}

