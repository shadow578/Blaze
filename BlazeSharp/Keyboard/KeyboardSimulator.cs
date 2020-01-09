using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BlazeSharp.Keyboard
{
    class KeyboardSimulator
    {
        /// <summary>
        /// Simulate a string beign typed
        /// </summary>
        /// <param name="str">the string to type</param>
        public void SendString(string str)
        {
            foreach (char c in str)
            {
                SendChar(c);
            }
        }

        /// <summary>
        /// simulate a character beign typed
        /// </summary>
        /// <param name="c">the char to type</param>
        public void SendChar(char c)
        {
            //get Key code from char
            Keys vkey = CharToVKey(c);

            //send keypress
            SendKey(vkey);
        }

        /// <summary>
        /// Send a simulated key globally
        /// </summary>
        /// <param name="key">the keys to simulate. supports bitwise- or-ing for Keys.SHIFT, ALT, and CTRL</param>
        /// <param name="keySleep">how long the key press is simulated (how long the key press lasts) (ms)</param>
        public void SendKey(Keys key, int keySleep = 0)
        {
            if (key.HasFlag(Keys.Control))
            {
                LowLevel.keybd_event((byte)Keys.LControlKey, 0, 0, 0);
            }

            if (key.HasFlag(Keys.Alt))
            {
                //LowLevel.keybd_event((byte)Keys.KeyCode, 0, 0, 0);
            }

            if (key.HasFlag(Keys.Shift))
            {
                LowLevel.keybd_event((byte)Keys.LShiftKey, 0, 0, 0);
            }

            if (keySleep > 0) Thread.Sleep(keySleep);
            LowLevel.keybd_event((byte)key, 0, 0, 0);
            if (keySleep > 0) Thread.Sleep(keySleep);
            LowLevel.keybd_event((byte)key, 0, LowLevel.Constants.KEYEVENTF_KEYUP, 0);

            if (key.HasFlag(Keys.Shift))
            {
                LowLevel.keybd_event((byte)Keys.LShiftKey, 0, LowLevel.Constants.KEYEVENTF_KEYUP, 0);
            }

            if (key.HasFlag(Keys.Alt))
            {
                //LowLevel.keybd_event((byte)Keys.KeyCode, 0, LowLevel.Constants.KEYEVENTF_KEYUP, 0);
            }

            if (key.HasFlag(Keys.Control))
            {
                LowLevel.keybd_event((byte)Keys.LControlKey, 0, LowLevel.Constants.KEYEVENTF_KEYUP, 0);
            }
        }

        /// <summary>
        /// Convert a character into a virtual key code
        /// </summary>
        /// <param name="ch">the char to convert</param>
        /// <returns>the virtual keycode. may have Keys.SHIFT, ALT, and CTRL set</returns>
        Keys CharToVKey(char ch)
        {
            short vkey = LowLevel.VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }

        /// <summary>
        /// Provides Low- Level Interop functionality
        /// </summary>
        internal static class LowLevel
        {
            internal static class Constants
            {
                public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
            }

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            [DllImport("user32.dll")]
            internal static extern short VkKeyScan(char ch);
        }
    }
}
