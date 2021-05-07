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
      public int light_sample = 0;
      bool isShiftDown = false;
      bool isCtrlDown = false;

      bool isOrthographic = false;
      bool doDrawGrid = false;

      public Game(int width, int height, string title) :
           base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {
         GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
         GL.Enable(EnableCap.DepthTest);
            //            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse,
            //new float[] { 1f, 0.2f, 0.2f, 1.0f });
            //            GL.Enable(EnableCap.Lighting);
            //            GL.Enable(EnableCap.Light0);
            //            GL.Enable(EnableCap.ColorMaterial);

            GL.Light(LightName.Light0, LightParameter.Position, new float[] { -5f, 10.0f, 10f });
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            // Use GL.Material to set your object's material parameters.
            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });

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

         GL.Enable(EnableCap.Lighting);
         GL.Enable(EnableCap.Light0);


         float[] lightPos = { 2, 0, 1, 1 };
         float[] material = { 0, 0, 0.9f, 1 };


         GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
         GL.Light(LightName.Light0, LightParameter.Position, lightPos);

         GL.Material(MaterialFace.Front, MaterialParameter.AmbientAndDiffuse, material);
         GL.Enable(EnableCap.ColorMaterial);

         //figure.DrawTexture();
         GL.Color3(1f, 0f, 0f);
         figure.DrawMesh();

         if (doDrawGrid)
         {
            GL.Color3(1f, 1f, 1f);
            GL.LineWidth(3f);
            figure.DrawGrid();
         }

         DrawCube(new Vector3(lightPos[0], lightPos[1], lightPos[2]), 0.5f);

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
                case Key.Z:
                    {
                        doDrawGrid = !doDrawGrid;
                        break;
                    }
                case Key.L:
                    {
                        if (light_sample == 8)
                        {
                            light_sample = 0;
                            Light_Mode();
                            break;
                        }
                        else
                        {
                            light_sample++;
                            Light_Mode();
                            break;
                        }
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
         if(isShiftDown)
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
         else if(isCtrlDown)
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

        void Light_Mode()
        {
            if (light_sample == 1)
            {
                GL.Disable(EnableCap.Light0);
                GL.Light(LightName.Light1, LightParameter.Position, new float[] { -5, 10.0f, -0.5f });
                GL.Light(LightName.Light1, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
                GL.Light(LightName.Light1, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light1, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light1, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light1);
            }
            if (light_sample == 2)
            {
                GL.Disable(EnableCap.Light1);
                GL.Light(LightName.Light2, LightParameter.Position, new float[] { -5, 10.0f, -0.5f });
                GL.Light(LightName.Light2, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
                GL.Light(LightName.Light2, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light2, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light2, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light2);
            }
            if (light_sample == 3)
            {
                GL.Disable(EnableCap.Light2);
                GL.Light(LightName.Light3, LightParameter.Position, new float[] { -5, 10.0f, -0.5f });
                GL.Light(LightName.Light3, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
                GL.Light(LightName.Light3, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light3, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light3, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light3);
            }
            if (light_sample == 4)
            {
                GL.Disable(EnableCap.Light3);
                GL.Light(LightName.Light4, LightParameter.Position, new float[] { -5, 10.0f, -0.5f });
                GL.Light(LightName.Light4, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
                GL.Light(LightName.Light4, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light4, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light4, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light4);
            }
            if (light_sample == 5)
            {
                GL.Disable(EnableCap.Light4);
                GL.Light(LightName.Light5, LightParameter.Position, new float[] { -5, 10.0f, -0.5f });
                GL.Light(LightName.Light5, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
                GL.Light(LightName.Light5, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light5, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light5, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light5);
            }
            if (light_sample == 6)
            {
                GL.Disable(EnableCap.Light5);
                GL.Light(LightName.Light6, LightParameter.Position, new float[] { -5, 10.0f, -0.5f });
                GL.Light(LightName.Light6, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
                GL.Light(LightName.Light6, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light6, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Light(LightName.Light6, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
                GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light6);
            }

            if (light_sample == 7)
            {
                GL.Disable(EnableCap.Light6);
                GL.Disable(EnableCap.Lighting);
            }
        }
    }
}
