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

      public Game(int width, int height, string title) :
           base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {


         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         Title = mouseX.ToString() + " " + mouseY.ToString();

         UpdatePhysics();
         Render();

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      private void UpdatePhysics()
      {


      }

      private void Render()
      {
         GL.ClearColor(Color4.DarkGray);

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
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         

         mouseX = e.X;
         mouseY = e.Y;

         base.OnMouseMove(e);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
        
         base.OnMouseWheel(e);
      }

      protected override void OnResize(EventArgs e)
      {
         GL.Disable(EnableCap.DepthTest);
         GL.Viewport(0, 0, Width, Height);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0, Width, Height, 0, -1.0, 1.0);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadIdentity();

         base.OnResize(e);
      }
   }
}
