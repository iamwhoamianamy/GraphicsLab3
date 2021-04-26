using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GraphicsLab3
{
   class Figure
   {
      public Vector3[] sideVertices;
      public Vector2[][] textureCoords;
      public Face[] sideFaces;
      public int polygonBase = 0;
      public float radius;

      public int divisions = 0;
      public Vector3[] trajectory;
      public Vector3 initialPoint;

      Bitmap bitmap;
      int textureID;

      public Figure()
      {
         sideVertices = new Vector3[0];
      }

      public void InitFigure(string fileName)
      {
         using (StreamReader sr = new StreamReader(fileName))
         {
            string l = sr.ReadLine();
            string[] parameters = l.Split(' ');

            polygonBase = int.Parse(parameters[0]);
            radius = int.Parse(parameters[1]);

            l = sr.ReadLine();
            divisions = int.Parse(l);
            trajectory = new Vector3[divisions];

            for (int i = 0; i < divisions; i++)
            {
               l = sr.ReadLine();
               parameters = l.Split(' ');
               trajectory[i] = new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]));

            }
         }

         sideVertices = new Vector3[polygonBase * divisions];

         sideFaces = new Face[2 * polygonBase * (divisions - 1)];
         for (int i = 1; i < polygonBase + 1; i++)
         {
            int nextV = (i + 1) % (polygonBase + 1);
            if (nextV == 0)
               nextV++;
         }

         Vector3[] polygon = CreatePolygon(trajectory[0], radius, polygonBase);
         for (int j = 0; j < polygonBase; j++)
            sideVertices[j] = polygon[j];

         Replication();
      }
      
      public void DrawMesh()
      {
         GL.Begin(BeginMode.Triangles);

         for (int i = 0; i < sideFaces.Length; i++)
         {
            GL.Vertex3(sideVertices[sideFaces[i].v0]);
            GL.Vertex3(sideVertices[sideFaces[i].v1]);
            GL.Vertex3(sideVertices[sideFaces[i].v2]);
         }

         GL.End();
      }

      public void DrawGrid()
      {
         GL.Begin(BeginMode.Lines);

         for (int i = 0; i < sideFaces.Length; i++)
         {
            GL.Vertex3(sideVertices[sideFaces[i].v0]);
            GL.Vertex3(sideVertices[sideFaces[i].v1]);

            GL.Vertex3(sideVertices[sideFaces[i].v0]);
            GL.Vertex3(sideVertices[sideFaces[i].v2]);

            GL.Vertex3(sideVertices[sideFaces[i].v1]);
            GL.Vertex3(sideVertices[sideFaces[i].v2]);
         }
         GL.End();
      }

      public void Replication()
      {
         for (int i = 1; i < divisions; i++)
         {
            Vector3[] polygon = CreatePolygon(trajectory[i], radius, polygonBase);

            if (i > 0 && i < divisions - 1)
            {

               Vector3 prevVec = Vector3.Zero;
               Vector3 nextVec = Vector3.Zero;

               prevVec = trajectory[i - 1] - trajectory[i];
               nextVec = trajectory[i + 1] - trajectory[i];

               Vector2 v1 = new Vector2(prevVec.Z, prevVec.Y);
               Vector2 v2 = new Vector2(nextVec.Z, nextVec.Y);
               float angle = MathHelper.PiOver2 - Help.AngleBetween(v1, v2) / 2;

               float ang = MathHelper.RadiansToDegrees(angle);

               if (float.IsNaN(angle))
                  angle = MathHelper.PiOver4;
               else if (angle == MathHelper.PiOver4 && v1.Y == v2.Y)
                  angle = 0;

               if (angle != 0)
               {
                  for (int j = 0; j < polygon.Length; j++)
                  {
                     polygon[j] -= trajectory[i];
                     polygon[j] = Help.RotateAroundX(polygon[j], angle * (v1.Y >= v2.Y ? 1 : -1));
                     polygon[j] += trajectory[i];
                  }
               }
            }

            for (int j = 0; j < polygonBase; j++)
            {
               sideVertices[polygonBase * i + j] = polygon[j];
            }
         }

         int faceStart = 0;

         for (int i = 0; i < divisions - 1; i++)
         {
            for (int j = 0; j < polygonBase; j++)
            {
               int v0 = j + i * polygonBase;
               int v1 = polygonBase + j + i * polygonBase;
               int v2 = polygonBase + ((j + 1) % polygonBase) + i * polygonBase;

               sideFaces[faceStart++] = new Face(v0, v1, v2);

               v1 = ((j + 1) % polygonBase) + i * polygonBase;

               sideFaces[faceStart++] = new Face(v0, v1, v2);
            }
         }      
      }

      public void BindTexture()
      {
         GL.GenTextures(1, out textureID);

         GL.BindTexture(TextureTarget.Texture2D, textureID);

         BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
           ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

         GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
             OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

         bitmap.UnlockBits(data);

         //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 6);

         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      }

      public void CalcTextureCoords()
      {
         textureCoords = new Vector2[divisions][];

         for (int i = 0; i < divisions; i++)
            textureCoords[i] = new Vector2[polygonBase];

         int start;

         if (polygonBase == 3 || polygonBase == 4)
            start = 0;
         else
            start = 1;

         float[] lengthes = new float[polygonBase];

         for (int i = 0; i < polygonBase; i++)
         {
            for (int j = 0; j < divisions; j++)
            {

            }
         }


         for (int i = 0; i < divisions; i++)
         {
            for (int j = 0; j < polygonBase; j++)
            {
               int v0 = start + j + i * polygonBase;
               int v1 = start + polygonBase + j + i * polygonBase;
               int v2 = start + polygonBase + ((j + 1) % polygonBase) + i * polygonBase;
               
               v1 = start + ((j + 1) % polygonBase) + i * polygonBase;

            }
         }


      }
      public void DrawTexture()
      {
         GL.BindTexture(TextureTarget.Texture2D, textureID);

         int faceStart;

         if (polygonBase == 3)
            faceStart = 1;
         else if (polygonBase == 4)
            faceStart = 2;
         else
            faceStart = polygonBase;

         GL.Enable(EnableCap.Texture2D);
         GL.Begin(BeginMode.Triangles);

         for (int i = 0; i < divisions - 1; i++)
         {
            for (int j = 0; j < polygonBase; j++)
            {
               GL.TexCoord2(textureCoords[i][j]);
               GL.Vertex3(sideVertices[sideFaces[faceStart].v0]);

               GL.TexCoord2(textureCoords[i + 1][j]);
               GL.Vertex3(sideVertices[sideFaces[faceStart].v1]);

               GL.TexCoord2(textureCoords[i + 1][(j + 1) % polygonBase]);
               GL.Vertex3(sideVertices[sideFaces[faceStart++].v2]);

               GL.TexCoord2(textureCoords[i][j]);
               GL.Vertex3(sideVertices[sideFaces[faceStart].v0]);

               GL.TexCoord2(textureCoords[i][(j + 1) % polygonBase]);
               GL.Vertex3(sideVertices[sideFaces[faceStart].v1]);

               GL.TexCoord2(textureCoords[i + 1][(j + 1) % polygonBase]);
               GL.Vertex3(sideVertices[sideFaces[faceStart++].v2]);
            }
         }

         GL.End();

         //GL.Begin(BeginMode.Triangles);

         //for (int i = 0; i < faces.Length; i++)
         //{
         //   GL.TexCoord2(0, 0);
         //   GL.Vertex3(vertices[faces[i].v0]);
         //   GL.TexCoord2(0, 1);
         //   GL.Vertex3(vertices[faces[i].v1]);
         //   GL.TexCoord2(1, 0);
         //   GL.Vertex3(vertices[faces[i].v2]);
         //}

         //GL.End();
         GL.Disable(EnableCap.Texture2D);
      }

      public void ReadTexture(string fileName)
      {
         bitmap = new Bitmap(fileName);

         //data = new int[bitmap.Height][];

         //for (int i = 0; i < bitmap.Height; i++)
         //   data[i] = new int[bitmap.Width];


         //for (int i = 0; i < bitmap.Height; i++)
         //{
         //   for (int j = 0; j < bitmap.Width; j++)
         //   {
         //      byte r = bitmap.GetPixel(j, i).R;
         //      byte g = bitmap.GetPixel(j, i).G;
         //      byte b = bitmap.GetPixel(j, i).B;
         //      byte a = bitmap.GetPixel(j, i).A;

         //      data[i][j] = r << 24 | g << 16 | b << 8 | a;
         //   }
         //}
      }

      public Vector3[] CreatePolygon(Vector3 pos, float radius, int polygonBase)
      {
         Vector3[] res = new Vector3[polygonBase];
         double angle = 2.0 / polygonBase * Math.PI;
         double addAngle = 0;

         if (polygonBase == 3)
            addAngle = -angle / 4;
         else if (polygonBase == 4)
            addAngle = angle / 2;

         for (int i = 0; i < polygonBase; i++)
            res[i] = new Vector3(pos.X + radius * (float)Math.Cos(angle * i + addAngle),
                                 pos.Y + radius * (float)Math.Sin(angle * i + addAngle), pos.Z);

         return res;
      }
   }

   class Face
   {
      public int v0;
      public int v1;
      public int v2;

      public Face(int v0, int v1, int v2)
      {
         this.v0 = v0;
         this.v1 = v1;
         this.v2 = v2;
      }
   }
}
