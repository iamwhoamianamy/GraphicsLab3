using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLab3
{
   class Program
   {
      public static void Main()
      {
         using (Game game = new Game(800, 800, "Geometric floppa"))
         {

            game.Run(60.0);
         }
      }
   }
}
