using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlazeSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            KeyboardMonitor mon = new KeyboardMonitor();
            mon.Init();
            mon.KeyPressed += Mon_KeyPressed;

            Console.WriteLine("RUNNING");
            Application.Run();

            mon.Dispose();
        }

        private static void Mon_KeyPressed(char vChar, Keys vKey)
        {
            Console.WriteLine(vKey + " - " + vChar);
        }
    }
}
