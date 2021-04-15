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
   class Figure
   {
      
      public Vector3[] vertices;
      public Face[] faces;
      int polygonBase = 0;
      float radius;

      int replicCount = 0;
      Vector3[] trajectory;

      public Figure()
      {
         vertices = new Vector3[0];
      }

      public void InitFigure(string fileName)
      {
         Vector3 pos;

         using (StreamReader sr = new StreamReader(fileName))
         {
            string l = sr.ReadLine();
            string[] parameters = l.Split(' ');

            pos = new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]));

            l = sr.ReadLine();
            parameters = l.Split(' ');

            polygonBase = int.Parse(parameters[0]);
            radius = int.Parse(parameters[1]);

            l = sr.ReadLine();
            replicCount = int.Parse(l);
            trajectory = new Vector3[replicCount];

            for (int i = 0; i < replicCount; i++)
            {
               l = sr.ReadLine();
               parameters = l.Split(' ');
               trajectory[i] = new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]));

            }
         }

         Vector3[] polygon = CreatePolygon(pos, radius, polygonBase);

         int start;
         if (polygonBase == 3 || polygonBase == 4)
            start = 0;
         else
            start = 1;

         vertices = new Vector3[polygonBase * (replicCount + 1) + start * 2];

         for (int i = 0; i < polygonBase; i++)
            vertices[i + start] = polygon[i];

         if (polygonBase != 3 && polygonBase != 4)
         {
            vertices[0] = pos;
            vertices[vertices.Length - 1] = trajectory[trajectory.Length - 1];
         }

         switch (polygonBase)
         {
            case 3:
            {
               faces = new Face[2 + replicCount * 6];
               faces[0] = new Face(0, 1, 2);
               break;
            }
            case 4:
            {
               faces = new Face[4 + replicCount * 8];
               faces[0] = new Face(0, 1, 2);
               faces[1] = new Face(2, 3, 0);
               break;
            }
            default:
            {
               faces = new Face[2 * polygonBase * (1 + replicCount)];
               for (int i = 1; i < polygonBase + 1; i++)
               {
                  int nextV = (i + 1) % (polygonBase + 1);
                  if (nextV == 0)
                     nextV++;

                  faces[i - 1] = new Face(0, i, nextV);
               }
               break;
            }
         }

         Replication();
      }
      
      public void DrawMesh()
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

      public void DrawGrid()
      {
         GL.Begin(BeginMode.Lines);

         for (int i = 0; i < faces.Length; i++)
         {
            GL.Vertex3(vertices[faces[i].v0]);
            GL.Vertex3(vertices[faces[i].v1]);

            GL.Vertex3(vertices[faces[i].v0]);
            GL.Vertex3(vertices[faces[i].v2]);

            GL.Vertex3(vertices[faces[i].v1]);
            GL.Vertex3(vertices[faces[i].v2]);
         }
         GL.End();
      }

      public void Replication()
      {
         int start;

         if (polygonBase == 3 || polygonBase == 4)
            start = 0;
         else
            start = 1;

         int faceStart;

         for (int i = 0; i < replicCount; i++)
         {
            Vector3[] polygon = CreatePolygon(trajectory[i], radius, polygonBase);

            for (int j = 0; j < polygonBase; j++)
            {
               vertices[start + polygonBase * (i + 1) + j] = polygon[j];
            }
         }

         if (polygonBase == 3)
            faceStart = 1;
         else if (polygonBase == 4)
            faceStart = 2;
         else
            faceStart = polygonBase;

         for (int i = 0; i < replicCount; i++)
         {
            for (int j = 0; j < polygonBase; j++)
            {
               int v0 = start + j + i * polygonBase;
               int v1 = start + polygonBase + j + i * polygonBase;
               int v2 = start + polygonBase + ((j + 1) % polygonBase) + i * polygonBase;

               faces[faceStart++] = new Face(v0, v1, v2);

               v1 = start + ((j + 1) % polygonBase) + i * polygonBase;

               faces[faceStart++] = new Face(v0, v1, v2);
            }
         }

         switch (polygonBase)
         {
            case 3:
            {
               faces[faces.Length - 1] = new Face(vertices.Length - 3, vertices.Length - 2, vertices.Length - 1);
               break;
            }
            case 4:
            {
               faces[faces.Length - 2] = new Face(vertices.Length - 1, vertices.Length - 2, vertices.Length - 3);
               faces[faces.Length - 1] = new Face(vertices.Length - 1, vertices.Length - 4, vertices.Length - 3);
               break;
            }
            default:
            {
               int startVert = start + polygonBase * trajectory.Length;

               for (int i = 0; i < polygonBase; i++)
               {
                  int next = (i + 1) % (polygonBase);
                  faces[faces.Length - polygonBase + i] = new Face(vertices.Length - 1, startVert + i, startVert + next);
               }
               break;
            }
         }

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
