using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CDLC_Manager.Settings
{

    internal static class Settings_Manager
    {


        //setting appdata path as null so in the settings function I can do a nullcheck to falesafe it.



        static string? AppDataPath = null;
        static string? configPath = null;
        static string? dlcFolder = null;
        static string? cdlcFolder = null;
        const string ErrorNull = "Attempted to return a null path";
        //This list object is here so that I can store the current DLC and CDLC filenames in one place, this is a memory whore moment
        //where the amount of data should only ever at most be a couple thousand strings it SHOULD be fine, much more than that the game will crash anyway.

        public static List<string> dlcNames = new List<string>();

        // Settings Indices contain the Following
        // 0: Download folder
        // 1: RS Directory, target folder
        // 2: Save Originals
        // 3: AutoSort
        // 4: Backup Directory
        // 5: Make Backup Checkbox Status
        public static string[] settings = new string[] { "", "", "0", "0", "", "0" };
        public static readonly string dlcAppend = "\\dlc\\cdlc";

        //We're going to check for a setting file in AppData, for out program, If they don't exist, we're going to create them.
        public static void checkForSettings(RichTextBox text, CheckBox cbSaveOrig, CheckBox cbAuto, Button btnTransfer, CheckBox cbMakeBackup)
        {
            //before we get started we want the local AppData path, this uses the System Namespace.
            //This will allow us to set the destination in a nice folder there. So let's do that first.
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //Now we create a variable with the "company" name for this I'm using OMD during development, Using caps in a directory is inherently a windows thing, Linux cares more so the convention is don't us caps or space on Linux filesystems. This is a strictly windows APP so it's fine to do caps)
            string orgFolder = FolderPath + "\\OMD";

            //DEBUG
            //print("DEBUG: AppdataPath: " + FolderPath);
            //print("DEBUG: ORG Path: " + orgFolder);
            //good, now we check if the path exists.

            //now the try catch block to check if the file exists, If it does, return, if it doesn't, create it, if creation fails, throw exception error"
            try
            {
                //Does the folder exist, Directory functions belong to Namespace System.IO
                if (Directory.Exists(orgFolder))
                {
                    Helpers.DataHelpers.print("Settings Folder Exists", text);
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(orgFolder);
                    //print to the RTB
                    //first get the Date/Time as string. This is a pain in the ass and requires Globalization Namespace to make it more "Friendly" but less friendly to coding, To make things easier for localization later though we SHOULD do it this way. I'm going to put the date time function in another function, then just return the result here.
                    string Time = Helpers.DataHelpers.ConvertDate(Directory.GetCreationTime(orgFolder));
                    string debugMessage = orgFolder + " was created at " + Time + "\n";
                    Helpers.DataHelpers.print(debugMessage, text);
                }
            }
            //now the catch block
            catch (Exception e)
            {
                Helpers.DataHelpers.print("DEBUG EXCEPTION: " + e.ToString(), text);
            }
            //Finally block in this case is empty
            finally
            {

            }
            AppDataPath = orgFolder;

            //time not to get or create the settings file
            //we'll set the file to config.txt remember you use a double \\ to denote a backslack in a string rather than an escape sequence
            configPath = orgFolder + "\\config.txt";
            //print(configPath);
            if (File.Exists(configPath))
            {
                Helpers.DataHelpers.print("Config Files Exist. Attempting to restore previous settings", text);
                readInSettings(configPath, text, cbSaveOrig, cbAuto, btnTransfer, cbMakeBackup);
            }
            else
            {
                //file creation time. This is a bit of a weird block, the file should exist, and we aren't adding anything to it which means empty block braces.
                using (File.Create(configPath))
                {

                }
                Helpers.DataHelpers.print("Please set Directories to use before continueing", text);
            }

        }

        private static void readInSettings(string filePath, RichTextBox text, CheckBox cbSaveOrig, CheckBox cbAuto, Button btnTransfer, CheckBox cbMakeBackup)
        {
            //read in the file by each line, line 0 will be the Download path, line 1 will be the RS path, it will add them to the settings list.
            int c = 0;
            foreach (string line in File.ReadLines(filePath))
            {
                if (c < 6)
                {
                    settings[c] = line;
                }
                else
                {
                    Helpers.DataHelpers.print("Hmm It seems there are more than 4 lines in settings, that shouldn't happen. Please check into that.", text);
                }
                c++;

            }
            foreach (string s in settings)
            {
                //print(s);
            }
            if (settings[0] == "")
            {
                Helpers.DataHelpers.print("The Download Directory is not set", text);
            }
            if (settings[1] == "" || settings[1] == null)
            {
                Helpers.DataHelpers.print("The RS Directory is not set.", text);
            }
            //print(settings[2]);
            //print(settings[3]);
#pragma warning disable IDE0075 // Can be simplified, I find this more readable.
            cbSaveOrig.IsChecked = settings[2] == "1" ? true : false;
            cbAuto.IsChecked = settings[3] == "1" ? true : false;
            cbMakeBackup.IsChecked = settings[5] == "1" ? true : false;
#pragma warning restore IDE0075 // Simplify conditional expression



            checkDlcDirectories(text, dlcAppend);
            checkReady(btnTransfer);
        }


        private static void writeSettings(Button btnTransfer, RichTextBox text)
        {
            if (configPath != null)
            {
                using (TextWriter tw = new StreamWriter(configPath))
                {
                    tw.WriteLine(settings[0]);
                    tw.WriteLine(settings[1]);
                    tw.WriteLine(settings[2]);
                    tw.WriteLine(settings[3]);
                    tw.WriteLine(settings[4]);
                    tw.WriteLine(settings[5]);
                    //print(settings[2]);
                    //print(settings[3]);
                    Helpers.DataHelpers.print("DEBUG: Configuration updated", text);
                }
                //while we are here since this is called any time the directories are set, this is a good time to check if the DLC folder exists, if it does, does a CDLC folder exist inside of it.

            }
            else
            {
                Helpers.DataHelpers.print("DEBUG: CONFIG PATH IS NULL", text);
            }
            if (settings[0] != "" && settings[1] != "")
            {
                //btnTransfer.Enabled = true;
            }
            checkReady(btnTransfer);
        }

        private static void checkReady(Button transfer)
        {
            if (settings[0] != "" && settings[1] != "")
            {
                if (Directory.Exists(cdlcFolder))
                {
                    //transfer.Enabled = true;
                }
            }
        }

        private static void checkDlcDirectories(RichTextBox text, string dlcAppend)
        {
            if (settings[1] != "")
            {
                dlcFolder = settings[1] + "\\dlc";
                cdlcFolder = settings[1] + dlcAppend;
                if (Directory.Exists(dlcFolder))
                {
                    Helpers.DataHelpers.print("DLC folder has been located. Checking for cdlc folder.", text);
                    if (Directory.Exists(cdlcFolder))
                    {
                        Helpers.DataHelpers.print("cdlc folder has been located.", text);

                        //gonna make the list for the DLCs we have already here.
                        dlcNames = Helpers.DataHelpers.populateDlcFilesList(dlcFolder, cdlcFolder);
                    }
                    else
                    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                        DirectoryInfo di = Directory.CreateDirectory(settings[1] + dlcAppend);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                        string Time = Helpers.DataHelpers.ConvertDate(Directory.GetCreationTime(settings[1] + dlcAppend));
                        string debugMessage = "DEBUG: " + settings[1] + dlcAppend + " was created at " + Time + "\n";
                        Helpers.DataHelpers.print(debugMessage, text);
                    }
                }
                else
                {
                    Helpers.DataHelpers.print("HOLDUP: Check your RS Directory, the DLC folder could not be found.", text);
                    Helpers.DataHelpers.print("This folder is created by default in a RS install. Either the directory is incorrect, or the installation may be damaged", text);
                }
            }
            else
            {
                Helpers.DataHelpers.print("RS Directory not set", text);
            }
        }
        public static string getCdlcFolder()
        {
            if (cdlcFolder != null)
            {
                return cdlcFolder; 
            }
            return ErrorNull;
        }
        public static string getAppDataPath()
        {
            if (AppDataPath != null)
            {
                return AppDataPath; 
            }
            return ErrorNull;
        }
        public static string getDlcFolder()
        {
            if (dlcFolder != null)
            {
                return dlcFolder; 
            }
            return ErrorNull;
        }
        public static string getConfigPath()
        {
            if (configPath != null)
            {
                return configPath; 
            }
            return ErrorNull;
        }
        public static string getBackupPath()
        {
            return settings[4] + "\\cdlc";
        }
        public static void requestWriteSettings(Button btnTransfer, RichTextBox text)
        {
            writeSettings(btnTransfer, text);
        }
    }
}
