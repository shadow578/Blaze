using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlazeSharp
{
    class App : ApplicationContext
    {
        #region Initialize App
        public static App Instance;

        static void Main(string[] args)
        {
            //create & start app instance
            Instance = new App();
            Instance.Init();
            Application.Run(Instance);

            //dispose app after running
            Instance.Dispose(true);
        }
        #endregion

        /// <summary>
        /// Keyboard Simulator to type
        /// </summary>
        KeyboardSimulator keySimulator;

        /// <summary>
        /// Keyboard monitor to capture typed keys
        /// </summary>
        KeyboardMonitor keyMonitor;

        /// <summary>
        /// String builder to capture commands that have been typed
        /// </summary>
        StringBuilder commandCapture;

        /// <summary>
        /// flag that indicates that a command is beign captured currently
        /// </summary>
        bool commandCapturing = false;

        /// <summary>
        /// Flag that indicates that the program is currently typing (=ignore keypresses in this time)
        /// </summary>
        bool selfTyping = false;

        char hotChar = '#';

        StringDictionary commandsDict;

        /// <summary>
        /// Initialize the application
        /// </summary>
        void Init()
        {
            //create instances
            keySimulator = new KeyboardSimulator();
            keyMonitor = new KeyboardMonitor();
            commandCapture = new StringBuilder();
            commandsDict = new StringDictionary();

            //initialize monitor
            keyMonitor.Init();
            keyMonitor.KeyPressed += OnGlobalKeyPress;

            dbInit();
        }

        void dbInit()
        {
            commandsDict.Add("test", "Das ist ein Test- Text...");
            commandsDict.Add("abc", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        /// <summary>
        /// Event Handler for KeyPressed Event
        /// </summary>
        /// <param name="vKey">The raw VKey that was pressed</param>
        /// <param name="vChar">the decoded unicode char that was pressed</param>
        /// <returns>should the keypress be cancelled?</returns>
        bool OnGlobalKeyPress(Keys vKey, char vChar)
        {
            //ignore if currently typing
            if (selfTyping) return false;

            //write pressed key to console
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(vKey + " - " + vChar);

            //record keypresses when capturing
            if (commandCapturing)
            {
                //record keypress
                commandCapture.Append(vChar);

                Console.SetCursorPosition(0, 1);
                Console.WriteLine(commandCapture.ToString());

                if (CheckAndTypeCommand())
                {
                    return true;
                }
            }

            //hotchar was pressed, start listening
            if (vChar.Equals(hotChar))
            {
                commandCapture.Clear();
                commandCapturing = true;
            }

            //don't cancel event
            return false;
        }

        /// <summary>
        /// Check if the command capture matches a command
        /// </summary>
        /// <returns>was a command typed out? (keypress should be cancelled if true)</returns>
        bool CheckAndTypeCommand()
        {
            //check EVERY command key
            string capCmd = commandCapture.ToString();
            bool validCommandPart = false;
            foreach(string cmd in commandsDict.Keys)
            {
                //check if currently captured command could be (part of) a existing command
                if(cmd.StartsWith(capCmd, true, CultureInfo.InvariantCulture))
                {
                    validCommandPart = true;
                }

                //check if currently captured command is a existing command
                if(cmd.Equals(capCmd, StringComparison.InvariantCultureIgnoreCase))
                {
                    //is a command!
                    validCommandPart = true;

                    //type out command text
                    TypeCommand(cmd, commandsDict[cmd]);

                    //exit right- away, cancelling currrent keypress
                    return true;
                }
            }

            if (!validCommandPart)
            {
                //the current command capture is NOT part of a existing command, stop capturing
                commandCapturing = false;
            }

            //dont cancel keypress
            return false;
        }

        /// <summary>
        /// Type a commands text, removing the typed command first
        /// </summary>
        /// <param name="cmd">the command that was typed</param>
        /// <param name="cmdText">the command text to type</param>
        void TypeCommand(string cmd, string cmdText)
        {
            //set typing flag
            selfTyping = true;

            //remove typed command
            //the last char is NOT typed yet, but we also have to remove the hotchar
            //so we can just pretend to fully remove the command string...
            for(int ci = 0; ci < cmd.Length; ci++)
            {
                keySimulator.SendKey(Keys.Back);
            }
         
            //type the command text
            keySimulator.SendString(cmdText);

            //reset typing flag
            selfTyping = false;
        }

        /// <summary>
        /// Dispose the app
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //dispose keymon
            keyMonitor.Dispose();
        }
    }
}
