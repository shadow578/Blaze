using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BlazeSharp
{
    class KeyboardMonitor : IDisposable
    {
        /// <summary>
        /// Key pressed event, global + low level
        /// </summary>
        /// <remarks>param 1 is the decoded unicode char, param 2 is the raw Key pressed</remarks>
        public event Action<char, Keys> KeyPressed;

        /// <summary>
        /// The low- level hooks handle
        /// </summary>
        IntPtr hookPtr;

        /// <summary>
        /// Initialize the keyboard monitor
        /// </summary>
        public void Init()
        {
            hookPtr = RegisterHook(LLKeyboardCallback);
        }

        /// <summary>
        /// Dispose the keyboard monitor, unregistering the low- level hook
        /// </summary>
        public void Dispose()
        {
            if (hookPtr != IntPtr.Zero)
            {
                UnRegisterHook(hookPtr);
                hookPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Register a low level keyboard hook
        /// </summary>
        /// <param name="callback">the callback function to register for the hook</param>
        /// <returns>the handle of the registered hook</returns>
        IntPtr RegisterHook(LowLevel.LowLevelKeyboardProc callback)
        {
            using (Process proc = Process.GetCurrentProcess())
            using (ProcessModule procModule = proc.MainModule)
            {
                IntPtr modHandle = LowLevel.GetModuleHandle(procModule.ModuleName);
                return LowLevel.SetWindowsHookEx(LowLevel.Constantss.WH_KEYBOARD_LL, callback, modHandle, 0);
            }
        }

        /// <summary>
        /// Unregisters the given windows hook
        /// </summary>
        /// <param name="hookPrt">the handle of the hook to unregister</param>
        void UnRegisterHook(IntPtr hookPrt)
        {
            LowLevel.UnhookWindowsHookEx(hookPrt);
        }

        /// <summary>
        /// Get a unicode char from a key, according to current keyboard mappings
        /// See https://stackoverflow.com/questions/5825820/how-to-capture-the-character-on-different-locale-keyboards-in-wpf-c
        /// </summary>
        /// <param name="key">the key to convert</param>
        /// <returns>the converted char</returns>
        char GetCharFromKey(Keys key)
        {
            char ch = ' ';

            int virtualKey = (int)key;
            byte[] keyboardState = new byte[256];
            LowLevel.GetKeyboardState(keyboardState);

            uint scanCode = LowLevel.MapVirtualKey((uint)virtualKey, LowLevel.MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = LowLevel.ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }

        /// <summary>
        /// Low- Level Keyboard hook Callback
        /// </summary>
        IntPtr LLKeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)LowLevel.Constantss.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                //process keypress
                Keys vKey = (Keys)vkCode;
                char vChar = GetCharFromKey(vKey);

                //invoke event
                KeyPressed?.Invoke(vChar, vKey);
            }

            return LowLevel.CallNextHookEx(hookPtr, nCode, wParam, lParam);
        }

        /// <summary>
        /// Provides Low- Level Interop functionality
        /// </summary>
        internal static class LowLevel
        {
            internal static class Constantss
            {
                public const int WH_KEYBOARD_LL = 13;
                public const int WM_KEYDOWN = 0x0100;
            }

            internal enum MapType : uint
            {
                MAPVK_VK_TO_VSC = 0x0,
                MAPVK_VSC_TO_VK = 0x1,
                MAPVK_VK_TO_CHAR = 0x2,
                MAPVK_VSC_TO_VK_EX = 0x3,
            }

            [DllImport("user32.dll")]
            internal static extern int ToUnicode(
                uint wVirtKey,
                uint wScanCode,
                byte[] lpKeyState,
                [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
                int cchBuff,
                uint wFlags);

            [DllImport("user32.dll")]
            internal static extern bool GetKeyboardState(byte[] lpKeyState);

            [DllImport("user32.dll")]
            internal static extern uint MapVirtualKey(uint uCode, MapType uMapType);

            internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);
        }
    }
}
