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
   class Game : GameWindow
   {
      float mouseX = 0, mouseY = 0;
      float rotation = 0f;
      Vector2 mousePressedLoc;
      float loc = 2f;

      Vector3 cameraPosition;
      float cameraZoom;
      float cameraYRotation;
      float cameraERotation;

      public Game(int width, int height, string title) :
           base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {
         GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
         GL.Enable(EnableCap.DepthTest);

         cameraZoom = 2;
         cameraPosition = new Vector3(0, 0, cameraZoom);
         cameraYRotation = 90f * (float)Math.PI / 180f;
         cameraERotation = 90f * (float)Math.PI / 180f;
         base.OnLoad(e);
      }

      protected override void OnResize(EventArgs e)
      {
         GL.Viewport(0, 0, Width, Height);

         Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadMatrix(ref projection);

         base.OnResize(e);
      }
      protected override void OnRenderFrame(FrameEventArgs e)
      {
         UpdatePhysics();
         Render();
         base.OnRenderFrame(e);
      }

      private void UpdatePhysics()
      {
         //loc += 0.01f;
         rotation += 0.01f;
      }

      private void Render()
      {
         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

         Matrix4 modelview = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadMatrix(ref modelview);

         GL.Color3(1f, 0f, 0f);
         DrawCube(Vector3.Zero, 1f);

         GL.Begin(BeginMode.Triangles);

         GL.Color3(0f, 1f, 0f); GL.Vertex3(0, 1f, 0f);
         GL.Color3(1f, 0f, 0f); GL.Vertex3(1f, 0f, 0f);
         GL.Color3(0f, 0f, 1f); GL.Vertex3(0f, 0f, 0f);

         GL.End();

         SwapBuffers();
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         if(e.Shift)
         {
            switch(e.Key)
            {
               
            }
         }

         switch(e.Key)
         {
           
         }

         base.OnKeyDown(e);
      }

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         switch(e.Button)
         {
            case MouseButton.Left:
            {
               
               break;
            }
            case MouseButton.Right:
            {
               

               break;
            }
            case MouseButton.Middle:
            {
               mousePressedLoc = new Vector2(e.X, e.Y);
               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if(e.Mouse.IsButtonDown(MouseButton.Middle))
         {
            cameraYRotation += e.XDelta * 0.01f;
            cameraERotation -= e.YDelta * 0.01f;
            RecalcCameraPosition();
            //cameraPosition.X = cameraZoom * (float)Math.Cos(cameraYRotation);
            //cameraPosition.Z = cameraZoom * (float)Math.Sin(cameraYRotation);


         }

         mouseX = e.X;
         mouseY = e.Y;

         base.OnMouseMove(e);
      }

      private void RecalcCameraPosition()
      {
         cameraPosition.X = cameraZoom * (float)Math.Cos(cameraYRotation) * (float)Math.Sin(cameraERotation);
         cameraPosition.Z = cameraZoom * (float)Math.Sin(cameraYRotation) * (float)Math.Sin(cameraERotation);
         cameraPosition.Y = cameraZoom * (float)Math.Cos(cameraERotation);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         cameraZoom -= e.Delta * 0.1f;
         RecalcCameraPosition();


         base.OnMouseWheel(e);
      }

      private void DrawCube(Vector3 center, float width)
      {
         GL.Begin(BeginMode.Quads);

         GL.Vertex3(center.X - width / 2, center.Y, center.Z - width / 2);
         GL.Vertex3(center.X + width / 2, center.Y, center.Z - width / 2);
         GL.Vertex3(center.X + width / 2, center.Y, center.Z + width / 2);
         GL.Vertex3(center.X - width / 2, center.Y, center.Z + width / 2);

         GL.End();
      }
   }
}
