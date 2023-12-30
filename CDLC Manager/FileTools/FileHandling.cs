using CDLC_Manager.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDLC_Manager.Helpers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CDLC_Manager.FileTools
{
    internal class FileHandling : IDisposable
    {
        BackgroundWorker fileWorker = new BackgroundWorker();
        Brush errorColor = new SolidColorBrush(Colors.Red);
        Brush nonErrColor = new SolidColorBrush (Colors.Green);
        private bool disposedValue;
        //I'm not sure if there's a better way to do this, but for now this is going to work.
        RichTextBox resultTextBox;

        public FileHandling(RichTextBox resultTextBox)
        {
            fileWorker.DoWork += FileWorker_DoWork1;
            fileWorker.RunWorkerCompleted += FileWorker_RunWorkerCompleted;
            fileWorker.WorkerReportsProgress = true;
            fileWorker.WorkerSupportsCancellation = true;
            fileWorker.ProgressChanged += FileWorker_ProgressChanged;
            this.resultTextBox = resultTextBox;
        }

        private void FileWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                RichTextBox richTextBox = (RichTextBox)e.Result;
                if (richTextBox != null)
                {
                    PrintOnUI("Transfer operation completed successfully", nonErrColor, richTextBox);
                }
            }
        }

        private void FileWorker_DoWork1(object? sender, DoWorkEventArgs e)
        {
           e.Result = prepareTransfer(e);
        }


        private RichTextBox prepareTransfer(DoWorkEventArgs de)
        {

            object[]? parameters = de.Argument as object[];

            if (parameters != null && parameters.Length == 4)
            {
                RichTextBox? text = parameters[0] as RichTextBox;
                CheckBox? cbSaveOrig = parameters[1] as CheckBox;
                CheckBox? cbAutoSort = parameters[2] as CheckBox;
                CheckBox? cbMakeBackup = parameters[3] as CheckBox;
                if (text != null && cbSaveOrig != null && cbAutoSort != null && cbMakeBackup != null)
                {
                    bool saveOriginal = false;
                    bool AutoSort = false;
                    bool MakeBackup = false;
                    cbSaveOrig.Dispatcher.Invoke(() => { saveOriginal = cbSaveOrig.IsChecked == true; });
                    cbAutoSort.Dispatcher.Invoke(() => { AutoSort = cbAutoSort.IsChecked == true; });
                    cbMakeBackup.Dispatcher.Invoke(() => { MakeBackup =  cbMakeBackup.IsChecked == true; });
                    string ext = "*.psarc";
                    try
                    {
                        //var files = Directory.EnumerateFiles(settings[0], ext, SearchOption.AllDirectories);
                        string[] files = Directory.GetFiles(SettingsManager.settings[0], ext);

                        //we now have a list of files, the first thing we need to do is check the drive to see if it's a basic move, or a copy op.
                        //I'm also going to check if the cbSaveOriginal is checked, I should actually put this all in a helper class.

                        if (Path.GetPathRoot(SettingsManager.settings[0]) == Path.GetPathRoot(SettingsManager.settings[1]) && cbSaveOrig.IsChecked != true)
                        {

                            foreach (string file in files)
                            {

                                string fileName = file.Substring(SettingsManager.settings[0].Length + 1);
                                //DataHelpers.print("FILENAME: " + fileName, text);
                                PrintOnUI("FILENAME: " + fileName, text);
                                string message = Path.Combine(SettingsManager.settings[0], file).ToString();
                                PrintOnUI("Preparing to move: " + message, text);
                                //if they are saving originals or have previously this can become a dangerous operation, Check the destination to see if the file exists.
                                if (!File.Exists(Path.Combine(SettingsManager.getCdlcFolder(), fileName)))
                                {
                                    Directory.Move(file, Path.Combine(SettingsManager.getCdlcFolder(), fileName));
                                    PrintOnUI("Moved: " + message, text);
                                }
                                else
                                {
                                    PrintOnUI("The file " + fileName + " Already exists in the destination, skipping move", errorColor, text);
                                }
                            }
                        }
                        else
                        {
                            //
                            PrintOnUI("Source and Destination are on different drives, Making copies then deleting source files", text);
                            foreach (string file in files)
                            {

                                string fileName = file.Substring(SettingsManager.settings[0].Length + 1);
                                PrintOnUI("FILENAME: " + fileName, text);
                                string message = Path.Combine(SettingsManager.settings[0], file).ToString();
                                PrintOnUI("Preparing to move: " + message, text);

                                //again check to make sure the file doesn't already exist


                                if (!File.Exists(Path.Combine(SettingsManager.getCdlcFolder(), fileName)))
                                {
                                    File.Copy(Path.Combine(SettingsManager.settings[0], fileName), Path.Combine(SettingsManager.getCdlcFolder(), fileName), true);
                                    //print("copied: " + message);
                                    //verify the file now exists in destination.
                                    if (File.Exists(Path.Combine(SettingsManager.getCdlcFolder(), fileName)))
                                    {
                                        PrintOnUI("File copied successfully", text);

                                    }
                                    else
                                    {
                                        PrintOnUI("FILE NOT COPIED, ABORTING", text);
                                        return text;
                                    }
                                }
                                else
                                {
                                    PrintOnUI("The file " + fileName + " Already exists in the destination, skipping move", errorColor, text);
                                }
                            }
                            if (!saveOriginal)
                            {
                                foreach (string f in files)
                                {
                                    PrintOnUI("Copy complete, Deleting source files.", text);
                                    File.Delete(f);
                                }
                            }
                        }

                        if (cbAutoSort != null)
                        {
                            if (AutoSort)
                            {
                                cleanup(text);
                            }
                        }
                        if (MakeBackup)
                        {
                            //MainProgramRef.RaiseAbortEvent += HandleAbortAsync;
                            //setupFileWorker();
                            if (!fileWorker.IsBusy)
                            {
                                fileWorker.RunWorkerAsync();
                            }


                        }
                    }

                    catch (Exception e)
                    {
                        PrintOnUI(e.ToString(), text);
                    }
                    return text;
                }
            }
            return resultTextBox;

        }

        private void MakeBackup(object sender, DoWorkEventArgs e)
        {
            //DataHelpers.print("Source Path is: " + Settings_Manager.getCdlcFolder(), Color.OrangeRed, text);
            //DataHelpers.print("Destination Path is: " + Settings_Manager.getBackupPath(), Color.OrangeRed, text);
            CopyDirectories(SettingsManager.getCdlcFolder(), SettingsManager.getBackupPath(), true, sender, e);
        }


        private void CopyDirectories(string sourceDirectory, string destinationDirectory, bool recursive, object sender, DoWorkEventArgs e)
        {
            DirectoryInfo SourceDir = new DirectoryInfo(sourceDirectory);
            DirectoryInfo DestDir = new DirectoryInfo(destinationDirectory);

            //check the source file
            if (!SourceDir.Exists)
            {
                MessageBox.Show("Tell the Dev The COPY Directories Function in FileHandling is not working correctly", "the dev fucked up", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DirectoryInfo[] subDirectories = SourceDir.GetDirectories();

            //check destination and make sure it exists

            try
            {
                if (!DestDir.Exists)
                {
                    DirectoryInfo di = Directory.CreateDirectory(DestDir.FullName);
                    //TextEventExtension TextEvent = new TextEventExtension(DestDir.FullName + " created successfully", Color.Green);
                    //string message = DestDir.FullName + " created successfully";
                    //fileWorker.ReportProgress(0, TextEvent);
                }
            }
            catch
            {
                MessageBox.Show("The Destination folder does not exist and could not be created. Is this a write protected directory (program files?)", "Could not create Destination", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            foreach (FileInfo file in SourceDir.GetFiles())
            {
                if (fileWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                string targetFilePath = Path.Combine(DestDir.FullName, file.Name);
                if (File.Exists(targetFilePath))
                {
                    System.Threading.Thread.Sleep(30);
                }
                else
                {
                    file.CopyTo(targetFilePath);
                }

            }

            //Now for the real test, recursion
            if (recursive)
            {
                foreach (DirectoryInfo subDir in subDirectories)
                {
                    if (fileWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                    CopyDirectories(subDir.FullName, newDestinationDir, true, sender, e);
                }
            }

        }

        private void cleanup(RichTextBox text)
        {
            string path = SettingsManager.settings[1] + SettingsManager.dlcAppend;
            string[] files = Directory.GetFiles(path, "*psarc");
            List<string> toDelete = new List<string>();
            foreach (string f in files)
            {
                string fileName = f.Substring(path.Length + 1);
                string[] split = fileName.Split('_');

                string bandName = split[0];
                PrintOnUI(bandName, text);
                string bandFolder = path + "\\" + bandName;
                if (Directory.Exists(bandFolder))
                {
                    if (!File.Exists(Path.Combine(bandFolder, fileName)))
                    {
                        Directory.Move(f, Path.Combine(bandFolder, fileName));
                    }
                    else
                    {
                        PrintOnUI("File Exists, Remove Skip move, and let it delete the extra", errorColor, text);
                        toDelete.Add(f);
                    }
                }
                else
                {

                    Directory.CreateDirectory(Path.Combine(path, bandName));
                    Directory.Move(f, Path.Combine(bandFolder, fileName));
                }


            }
            PrintOnUI("Cleaning up duplicate files", nonErrColor, text);
            foreach (string s in toDelete)
            {
                //PrintOnUI("Debug: path is: " + s, errorColor, text);
                File.Delete(s);
            }
            PrintOnUI("Cleanup Complete", nonErrColor, text);

        }



        public void ReqTransfer(RichTextBox text, CheckBox cbSaveOrig, CheckBox cbAutoSort, CheckBox cbMakeBackup)
        {
            if (!fileWorker.IsBusy)
            {
                fileWorker.RunWorkerAsync(new object[] { text, cbSaveOrig, cbAutoSort, cbMakeBackup });
            }
        }
        public void ReqCleanup(RichTextBox text)
        {
            cleanup(text);
        }
        //now that we are using a secondary thread for the transfer. I need to make a function to check access to the UI and then print on UI thread if needed
        private void PrintOnUI(string message, RichTextBox text)
        {
            if (text.Dispatcher.CheckAccess())
            {
                DataHelpers.print(message, text);
            }
            else
            {
                text.Dispatcher.Invoke(() => DataHelpers.print(message, text));
            }
        }
        private void PrintOnUI(string message, Brush color, RichTextBox text)
        {
            if (text.Dispatcher.CheckAccess())
            {
                DataHelpers.print(message, color, text);
            }
            else
            {
                text.Dispatcher.Invoke(() => DataHelpers.print(message, color, text));
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    fileWorker.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FileHandling()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        //public static void ReqBackup(CDLCReNamer MainProgramRef)
        //{
        //    MainProgramRef.RaiseAbortEvent += HandleAbortAsync;
        //    setupFileWorker();
        //    if (!fileWorker.IsBusy)
        //    {
        //        fileWorker.RunWorkerAsync();
        //    }
        //}
    }
}
