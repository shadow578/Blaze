using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace BlazeSharp
{
    class BlazeCommands
    {
        /// <summary>
        /// Hotchar that has to be typed to activate command capture
        /// </summary>
        public char HotChar { get; private set; } = '#';

        /// <summary>
        /// Commands dictionary
        /// </summary>
        public StringDictionary CommandsDict { get; private set; } = new StringDictionary();


        /// <summary>
        /// Load a blaze commands file
        /// </summary>
        /// <param name="path">the path to the .blaze file</param>
        /// <returns>the blaze commands in that file</returns>
        public static BlazeCommands FromFile(string path)
        {
            //check file exists
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            //read file line- by- line
            StringDictionary commandsDict = new StringDictionary();
            char? hotchar = null;
            using (StreamReader reader = File.OpenText(path))
            {
                string ln;
                bool inCommandBody = false;
                string commandName = null;
                StringBuilder commandBody = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    //read next line
                    ln = reader.ReadLine();

                    //skip if invalid
                    if (ln == null) continue;

                    //end keyword overrides command body...
                    if (ln.StartsWith("#end", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //remove last /r/n from command body
                        commandBody.Length -= Environment.NewLine.Length;

                        //check command name and body are ok
                        string cmdBody = commandBody.ToString();
                        if (!string.IsNullOrWhiteSpace(commandName) && !string.IsNullOrWhiteSpace(cmdBody))
                        {
                            //add command to dict if not already on list
                            if (!commandsDict.ContainsKey(commandName))
                            {
                                commandsDict.Add(commandName, cmdBody);
                            }
                        }

                        //reset command body flag and skip line
                        inCommandBody = false;
                        continue;
                    }

                    //handle when in command body
                    if (!inCommandBody)
                    {
                        //not in command body, handle keywords
                        if (ln.StartsWith("#hotchar", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //get hotchar after keyword
                            string hc = ln.ToLower().Replace("#hotchar", "").Trim();

                            //hotchar is first (+ only) char in string
                            if (hc.Length > 0)
                            {
                                hotchar = hc.First();
                            }
                        }
                        else if (ln.StartsWith("#command", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //get name of command
                            commandName = ln.ToLower().Replace("#command", "").Trim();

                            //set command body flag
                            inCommandBody = true;
                            commandBody.Clear();
                        }
                    }
                    else
                    {
                        //in command body, add lines raw
                        commandBody.AppendLine(ln);
                    }
                }
            }

            //check if parsing was successfull
            if (!hotchar.HasValue || commandsDict.Count <= 0)
            {
                //failed
                return null;
            }

            //build new commands list
            return new BlazeCommands()
            {
                HotChar = hotchar.Value,
                CommandsDict = commandsDict
            };
        }
    }
}
