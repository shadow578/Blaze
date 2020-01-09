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
        static KeyboardSimulator sim;
        static KeyboardMonitor mon;

        static void Main(string[] args)
        {
            Console.WriteLine($"Keys: {(int)Keys.W} - target: {0x52}");

            sim = new KeyboardSimulator();

            mon = new KeyboardMonitor();
            mon.Init();
            mon.KeyPressed += Mon_KeyPressed;

            Console.WriteLine("RUNNING");
            Application.Run();

            mon.Dispose();
        }

        private static bool Mon_KeyPressed(Keys vKey, char vChar)
        {
            Console.WriteLine(vKey + " - " + vChar);

            if (vChar == '#')
            {
                //sim.SendKey(Keys.G, true);
                sim.SendString("Test String OoO");
                return true;
            }

            return false;
        }
    }
}
