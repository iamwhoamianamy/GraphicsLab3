using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GraphicsLab3
{
   class Help
   {
      public static float AngleBetween(Vector2 vec1, Vector2 vec2)
      {
         //int sign = Math.Sign(Vector2.PerpDot(vec1, vec2)) * -1;
         //double dInDeg = MathHelper.RadiansToDegrees(d);
         return (float)Math.Acos(Vector2.Dot(vec1, vec2) / (vec1.Length * vec2.Length));
      }

      public static Vector3 RotateAroundX(Vector3 vec, float angle)
      {
         Vector3 res = Vector3.Zero;
         res.X = vec.X;
         res.Y = vec.Y * (float)Math.Cos(angle) - vec.Z * (float)Math.Sin(angle);
         res.Z = vec.Y * (float)Math.Sin(angle) + vec.Z * (float)Math.Cos(angle);

         return res;
      }

      public static Vector3 RotateAroundY(Vector3 vec, float angle)
      {
         Vector3 res = Vector3.Zero;
         res.X = vec.X * (float)Math.Cos(angle) + vec.Z * (float)Math.Sin(angle);
         res.Y = vec.Y;
         res.Z = -vec.X * (float)Math.Sin(angle) + vec.Z * (float)Math.Cos(angle);

         return res;
      }

      public static Vector3 RotateAroundZ(Vector3 vec, float angle)
      {
         Vector3 res = Vector3.Zero;
         res.X = vec.X * (float)Math.Cos(angle) - vec.Y * (float)Math.Sin(angle);
         res.Y = vec.X * (float)Math.Sin(angle) + vec.Y * (float)Math.Cos(angle);
         res.Z = vec.Z;

         return res;
      }


   }
}
