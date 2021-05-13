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
   public class Game : GameWindow
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
      bool drawGrid = false;
      bool drawNomrals = false;
      bool accountNormals = false;
      bool accountLightning = false;
      bool drawTexture = false;

      public string title = "Geometric floppa: ";

      public Game(int width, int height, string title) :
           base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {
         GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
         GL.Enable(EnableCap.DepthTest);

         // Use GL.Material to set your object's material parameters.

         ResetCameraPosition();

         figure = new Figure();
         figure.InitFigure("../../figure.txt");

         figure.ReadTexture("../../BigFloppa.png");
         figure.BindTexture();
         figure.CalcTextureCoords();
         figure.CalcNormals();

         //GL.Enable(EnableCap.AutoNormal);

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
         UpdateTitle();
         Render();

         base.OnRenderFrame(e);
      }

      private void UpdateTitle()
      {
         string newTitle = title;

         if (drawGrid)
            newTitle += "Grid: On; ";
         else
            newTitle += "Grid: Off; ";

         if (drawNomrals)
            newTitle += "Draw normals: On; ";
         else
            newTitle += "Draw normals: Off; ";

         if (accountNormals)
            newTitle += "Normals: On; ";
         else
            newTitle += "Normals: Off; ";

         if (accountLightning)
            newTitle += "Lightning: On; ";
         else
            newTitle += "Lightning: Off; ";

         if (drawTexture)
            newTitle += "Drawing: Texture; ";
         else
            newTitle += "Drawing: Raw color; ";

         Title = newTitle;
      }

      private void Render()
      {
         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

         Matrix4 modelview = Matrix4.LookAt(cameraShift + cameraPosition, cameraShift + Vector3.Zero, Vector3.UnitY);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadMatrix(ref modelview);


         if (accountLightning)
         {
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 0.1f, 0.5f, 0.1f, 1.0f });

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            AccountLightning();
         }
         else
         {
            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Light0);
         }

         if (drawTexture)
         {
            if (accountNormals)
            {
               figure.DrawTextureWithNormals();
            }
            else
            {
               figure.DrawTexture();
            }
         }
         else
         {
            if (accountNormals)
            {
               figure.DrawMeshWithNormals();
            }
            else
            {
               GL.Normal3(0.0f, 0.0f, 1.0f);
               figure.DrawMesh();
            }
         }

         if (drawGrid)
         {
            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Light0);

            GL.LineWidth(3f);
            GL.Color3(1f, 0f, 0f);
            figure.DrawGrid();
            GL.Color3(1f, 1f, 1f);
         }


         if (drawNomrals)
         {
            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Light0);

            GL.LineWidth(2f);
            GL.Color3(0f, 1f, 0f);
            figure.DrawNormals();
            GL.Color3(1f, 1f, 1f);
         }

         //DrawCube(new Vector3(lightPos[0], lightPos[1], lightPos[2]), 0.5f);

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
            case Key.G:
            {
               drawGrid = !drawGrid;
               break;
            }
            case Key.N:
            {
               accountNormals = !accountNormals;
               break;
            }
            case Key.D:
            {
               drawNomrals = !drawNomrals;
               break;
            }
            case Key.L:
            {
               accountLightning = !accountLightning;
               break;
            }
            case Key.T:
            {
               drawTexture = !drawTexture;
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
         switch (e.Button)
         {
            case MouseButton.Middle:
            {

               break;
            }
            case MouseButton.Right:
            {


               break;
            }
            case MouseButton.Left:
            {
               mousePressedLoc = new Vector2(e.X, e.Y);
               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (isShiftDown)
         {
            if (e.Mouse.IsButtonDown(MouseButton.Left))
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
         else if (isCtrlDown)
         {
            if (e.Mouse.IsButtonDown(MouseButton.Left))
            {
               cameraZoom += e.YDelta * 0.02f;
               RecalcCameraPosition();
            }
         }
         else if (e.Mouse.IsButtonDown(MouseButton.Left))
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

      void AccountLightning()
      {
         GL.Light(LightName.Light0, LightParameter.Position, new float[] { -3.0f, 3.0f, 0.5f });
         GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.5f, 0.2f, 0.2f, 1.0f });
         GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.1f, 0.1f, 0.5f, 1.0f });
         GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.1f, 0.1f, 0.1f, 1.0f });

         GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
         GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
      }

      
   }
}
