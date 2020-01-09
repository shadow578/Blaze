using BlazeSharp.Keyboard;
using BlazeSharp.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlazeSharp
{
    class App : ApplicationContext
    {
        /// <summary>
        /// The default title for notifications and the notify icon
        /// </summary>
        const string NOTIFY_TITLE = "Blaze";

        /// <summary>
        /// The default commands file path
        /// </summary>
        const string DEFAULT_COMMANDS_FILE = "./default_commands.blaze";

        /// <summary>
        /// the full path of the commands file for the current user
        /// </summary>
        string CommandsFile
        {
            get
            {
                //create path
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blaze", "commands.blaze");

                //make dirs as required
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                return path;
            }
        }

        #region Initialize App
        public static App Instance;

        [STAThread]
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
        /// The notify icon that provides basic user controls
        /// </summary>
        NotifyIcon notifyIcon;

        /// <summary>
        /// live stats ui. may be null
        /// </summary>
        LiveStatsUI liveStats;

        /// <summary>
        /// Keyboard Simulator to type
        /// </summary>
        KeyboardSimulator keySimulator;

        /// <summary>
        /// Keyboard monitor to capture typed keys
        /// </summary>
        KeyboardMonitor keyMonitor;

        /// <summary>
        /// contains all user- defined commands that were loaded
        /// </summary>
        BlazeCommands commands;

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

        /// <summary>
        /// Initialize the application
        /// </summary>
        void Init()
        {
            //create instances
            keySimulator = new KeyboardSimulator();
            keyMonitor = new KeyboardMonitor();
            commandCapture = new StringBuilder();

            //initialize ui
            liveStats = new LiveStatsUI();
            InitNotify();

            //load commands
            commands = BlazeCommands.FromFile(CommandsFile);
            if (commands == null)
            {
                //MessageBox.Show($"Could not load Commands from \n{CommandsFile}", "Cannot Load Commands", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();

                //load default commands
                File.Copy(DEFAULT_COMMANDS_FILE, CommandsFile);
                commands = BlazeCommands.FromFile(CommandsFile);
                SendNotification("Default Commands were loaded!");
            }

            //sanity- check: commands should be loaded by now
            if (commands == null)
            {
                return;
            }

            //initialize monitor
            keyMonitor.Init();
            keyMonitor.KeyPressed += OnGlobalKeyPress;

            AfterInit();
        }

        /// <summary>
        /// initializes the notify icon
        /// </summary>
        void InitNotify()
        {
            //create menu
            ContextMenu context = new ContextMenu();
            context.MenuItems.Add("Edit Commands", (ts, te) =>
            {
                //open commands.blaze file in editor
                Process editor = Process.Start("notepad", CommandsFile);
                editor.EnableRaisingEvents = true;
                editor.Exited += (xs, xe) =>
                {
                    //reload commands
                    commands = BlazeCommands.FromFile(CommandsFile);
                    SendNotification("Commands File was reloaded!");
                    editor.Dispose();
                };
            });
            context.MenuItems.Add("Autostart App", (s, e) =>
            {
                //get sender as menuitem
                MenuItem sender = s as MenuItem;
                if (sender == null) return;

                //toggle auto- start state
                bool autoEn = GetAutoStartEnabled();
                RegisterAutostart(!autoEn);
                sender.Checked = !autoEn;
            });
            context.MenuItems.Add("Live Stats", (s, e) =>
            {
                //show live stats window
                liveStats.Show();
            });
            context.MenuItems.Add("Exit App", (s, e) =>
            {
                //exit the app
                Application.Exit();
            });

            //create icon
            notifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.app_icon,
                Text = NOTIFY_TITLE,
                ContextMenu = context
            };

            //make icon visible
            notifyIcon.Visible = true;

            //allows notifications
            notifyIcon.BalloonTipClosed += (s, e) =>
            {
                notifyIcon.Visible = false;
                notifyIcon.Visible = true;
            };
        }

        /// <summary>
        /// Called after Init() finishes
        /// </summary>
        void AfterInit()
        {
            //get hotchar
            string strHotChar = commands.HotChar.ToString();

            //get commands as list
            List<string> cmds = commands.CommandsDict.Keys.Cast<string>().ToList();

            //get random command
            Random rnd = new Random();
            string strExampleCmd = cmds[rnd.Next(0, cmds.Count)];

            //fallback command to "NULL"
            if (string.IsNullOrWhiteSpace(strExampleCmd))
            {
                strExampleCmd = "NULL";
            }

            //send notification explaining how it works
            SendNotification($"{commands.CommandsDict.Count} Commands were loaded. \nOne of them is {strHotChar}{strExampleCmd}", "Blaze is now active");
        }

        /// <summary>
        /// Send a notification
        /// </summary>
        /// <param name="msg">the message of the notification</param>
        /// <param name="title">the title of the notification</param>
        void SendNotification(string msg, string title = NOTIFY_TITLE)
        {
            //dont continue if no notify icon
            if (notifyIcon == null) return;

            //send notification
            notifyIcon.ShowBalloonTip(5000, title, msg, ToolTipIcon.None);
        }

        /// <summary>
        /// Register this program for auto- start for the current user
        /// </summary>
        /// <param name="autoStart">should auto start be enabled or disabled?</param>
        void RegisterAutostart(bool autoStart)
        {
            //get autostart root key
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (autoStart)
            {
                regKey.SetValue(Application.ProductName, Application.ExecutablePath);
            }
            else
            {
                regKey.DeleteValue(Application.ProductName);
            }
        }

        bool GetAutoStartEnabled()
        {
            //get autostart root key
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return regKey.GetValue(Application.ProductName) != null;
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

            //record keypresses when capturing
            bool cancelKeypress = false;
            if (commandCapturing)
            {
                //record keypress
                commandCapture.Append(vChar);
                if (CheckAndTypeCommand())
                {
                    //command typed
                    cancelKeypress = true;
                    commandCapturing = false;
                    commandCapture.Clear();
                }
            }

            //hotchar was pressed, start listening
            if (vChar.Equals(commands.HotChar))
            {
                commandCapture.Clear();
                commandCapturing = true;
            }

            //write stats to ui
            liveStats?.UpdateStats(vKey, vChar, commandCapture.ToString(), commandCapturing);

            return cancelKeypress;
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
            foreach (string cmd in commands.CommandsDict.Keys)
            {
                //check if currently captured command could be (part of) a existing command
                if (cmd.StartsWith(capCmd, true, CultureInfo.InvariantCulture))
                {
                    validCommandPart = true;
                }

                //check if currently captured command is a existing command
                if (cmd.Equals(capCmd, StringComparison.InvariantCultureIgnoreCase))
                {
                    //is a command!
                    validCommandPart = true;

                    //type out command text
                    TypeCommand(cmd, commands.CommandsDict[cmd]);

                    //exit right- away, cancelling currrent keypress
                    return true;
                }
            }

            if (!validCommandPart)
            {
                //the current command capture is NOT part of a existing command, stop capturing
                commandCapturing = false;
                commandCapture.Clear();
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
            for (int ci = 0; ci < cmd.Length; ci++)
            {
                keySimulator.SendKey(Keys.Back);
            }

            //get current clipboard content (text)
            string clip = Clipboard.GetText();

            //copy the text to clipboard
            Clipboard.SetText(cmdText);

            //type CTRL+V to paste the clipboard
            keySimulator.SendKey(Keys.Control | Keys.V, 50);

            //restore clipboard
            Clipboard.SetText(clip);

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
            keyMonitor?.Dispose();

            //dispose uis
            liveStats?.Dispose();
            notifyIcon?.Dispose();
        }
    }
}
