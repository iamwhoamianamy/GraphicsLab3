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
      Figure figure;

      Vector3 cameraPosition;
      Vector3 cameraShift;
      float cameraZoom;
      float cameraYRotation;
      float cameraERotation;

      bool isShiftDown = false;
      bool isCtrlDown = false;

      bool isOrthographic = false;

      public Game(int width, int height, string title) :
           base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {
         GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
         GL.Enable(EnableCap.DepthTest);

         ResetCameraPosition();

         figure = new Figure();
         figure.InitFigure("../../figure.txt");

         figure.ReadTexture("../../BigFloppa.png");
         figure.BindTexture();
         figure.CalcTextureCoords();

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

         //if(isOrthographic)
         //{
         //   Matrix4 modelview = Matrix4.CreateOrthographicOffCenter(1f, -1f, -1f, 1f, -10f, 10f);
         //   GL.MatrixMode(MatrixMode.Modelview);
         //   GL.LoadMatrix(ref modelview);
         //   GL.Rotate(180, 0, 1f, 0);
         //   GL.Translate(0, 0, -11);

         //   GL.Translate(cameraShift + cameraPosition);
         //   GL.Scale(0.5f, 0.5f, 0.5f);

         //   //GL.Translate(cameraShift + cameraPosition);
         //}
         //else
         {
            Matrix4 modelview = Matrix4.LookAt(cameraShift + cameraPosition, cameraShift + Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
         }


         //figure.DrawTexture();

         GL.Color3(1f, 0f, 0f);
         figure.DrawMesh();

         GL.Color3(1f, 1f, 1f);
         GL.LineWidth(3f);
         figure.DrawGrid();

         //Vector3 v0 = new Vector3(0f, 0f, 2f);
         //v0 = RotateAroundY(v0, -cameraYRotation);

         //GL.Color3(0f, 1f, 0f);

         //GL.Begin(BeginMode.Lines);

         //GL.Vertex3(0f, 0f, 0f);
         //GL.Vertex3(v0);

         //GL.End();

         SwapBuffers();
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         if (e.Shift)
            isShiftDown = true;

         if (e.Control)
            isCtrlDown = true;

         switch (e.Key)
         {
            case Key.Slash:
            {
               ResetCameraPosition();
               break;
            }
            case Key.Keypad5:
            {
               isOrthographic = !isOrthographic;
               break;
            }
         }

         base.OnKeyDown(e);
      }

      protected override void OnKeyUp(KeyboardKeyEventArgs e)
      {
         if (!e.Shift)
            isShiftDown = false;
         if (!e.Control)
            isCtrlDown = false;

         base.OnKeyUp(e);
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
         if(isShiftDown)
         {
            if (e.Mouse.IsButtonDown(MouseButton.Middle))
            {

               Vector3 v0 = new Vector3(0f, 0f, 2f);
               v0 = Help.RotateAroundY(v0, -cameraYRotation);

               cameraShift.X += v0.X * e.XDelta * 0.005f;
               cameraShift.Z += v0.Z * e.XDelta * 0.005f;

               Vector3 v1 = new Vector3(0f, 0f, 2f);

               v1 = Help.RotateAroundX(v1, cameraERotation);
               v1 = Help.RotateAroundY(v1, -cameraYRotation + MathHelper.DegreesToRadians(90f));

               cameraShift.X -= v1.X * e.YDelta * 0.005f;
               cameraShift.Y -= v1.Y * e.YDelta * 0.005f;
               cameraShift.Z -= v1.Z * e.YDelta * 0.005f;
            }
         }
         else if(isCtrlDown)
         {
            if (e.Mouse.IsButtonDown(MouseButton.Middle))
            {
               cameraZoom += e.YDelta * 0.02f;
               RecalcCameraPosition();
            }
         }
         else if (e.Mouse.IsButtonDown(MouseButton.Middle))
         {
            cameraYRotation += e.XDelta * 0.01f;
            cameraERotation -= e.YDelta * 0.01f;
            RecalcCameraPosition();
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

      private void ResetCameraPosition()
      {
         cameraZoom = 4;
         cameraPosition = new Vector3(0, 0, -cameraZoom);
         cameraShift = Vector3.Zero;
         cameraYRotation = -90f * (float)Math.PI / 180f;
         cameraERotation = 90f * (float)Math.PI / 180f;
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         cameraZoom -= e.Delta * 0.2f;
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
