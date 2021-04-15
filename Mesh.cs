using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GraphicsLab3
{
   class Mesh
   {
      public Vector3[] vertices;
      public Face[] faces;

      public Mesh()
      {
         vertices = new Vector3[0];
      }

      public void InitFigure(string fileName)
      {
         int verticesCount;
         float radius;
         Vector3 pos;

         using (StreamReader sr = new StreamReader(fileName))
         {
            string l = sr.ReadLine();
            string[] parameters = l.Split(' ');

            pos = new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]));

           l = sr.ReadLine();
            parameters = l.Split(' ');

            verticesCount = int.Parse(parameters[0]);
            radius = int.Parse(parameters[1]);
         }

         switch (verticesCount)
         {
            case 3:
            {
               vertices = new Vector3[3];
               double angle = 2.0 / verticesCount * Math.PI;

               for (int i = 0; i < verticesCount; i++)
                  vertices[i] = new Vector3(pos.X + radius * (float)Math.Cos(angle * i - angle / 4), pos.Y + radius * (float)Math.Sin(angle * i - angle / 4), 0);

               break;
            }
            case 4:
            {
               vertices = new Vector3[4];
               double angle = 2.0 / verticesCount * Math.PI;

               for (int i = 0; i < verticesCount; i++)
                  vertices[i] = new Vector3(pos.X + radius * (float)Math.Cos(angle * i + angle / 2), pos.Y + radius * (float)Math.Sin(angle * i + angle / 2), 0);

               break;
            }
            default:
            {
               vertices = new Vector3[verticesCount + 1];
               double angle = 2.0 / verticesCount * Math.PI;
               vertices[0] = new Vector3(pos.X, pos.Y, pos.Z);

               for (int i = 1; i < verticesCount + 1; i++)
                  vertices[i] = new Vector3(pos.X + radius * (float)Math.Cos(angle * i + Math.PI / 2.0), pos.Y + radius * (float)Math.Sin(angle * i + Math.PI / 2.0), 0);

               break;
            }
         }
         
         switch(verticesCount)
         {
            case 3:
            {
               faces = new Face[1];
               faces[0] = new Face(0, 1, 2);
               break;
            }
            case 4:
            {
               faces = new Face[2];
               faces[0] = new Face(0, 1, 2);
               faces[1] = new Face(2, 3, 0);
               break;
            }
            default:
            {
               faces = new Face[verticesCount];
               for (int i = 1; i < verticesCount + 1; i++)
               {
                  int nextV = (i + 1) % (verticesCount + 1);
                  if (nextV == 0)
                     nextV++;

                  faces[i - 1] = new Face(0, i, nextV);
               }
               break;
            }
         }
      }
      
      public void Draw()
      {
         GL.Begin(BeginMode.Triangles);

         for (int i = 0; i < faces.Length; i++)
         {
            GL.Vertex3(vertices[faces[i].v0]);
            GL.Vertex3(vertices[faces[i].v1]);
            GL.Vertex3(vertices[faces[i].v2]);
         }

         GL.End();
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
