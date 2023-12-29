using CDLC_Manager.FileTools;
using CDLC_Manager.Settings;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;
using CDLC_Manager.Helpers;
namespace CDLC_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool auto = false;
        private FileSystemWatcher FS = new FileSystemWatcher();
        //private FileHandling fileHandler = new FileHandling();
        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, "Dark.Steel");
            SettingsManager.checkForSettings(richTextBox, SaveOrginalCB, AutoSortCB, TransferButton, AutoBackupCB);
            DLCFolderDisplayRTB.Document = DataHelpers.RTBDocuGen(SettingsManager.getDlcFolder());
            DownloadFolderDisplayRTB.Document = DataHelpers.RTBDocuGen(SettingsManager.settings[0]);
            TransferButton.Click += TransferButton_Click;
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            //FileHandling fileHandling = new FileHandling();
            FileHandling.ReqTransfer(richTextBox, SaveOrginalCB, AutoSortCB, AutoBackupCB);
            //fileHandling.
        }

        private void SetDLCFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SettingsManager.settings[1] = dialog.SelectedPath;
                DataHelpers.print("RS folder has been set to " + SettingsManager.settings[1], richTextBox);
                SettingsManager.requestWriteSettings(TransferButton, richTextBox);
                DLCFolderDisplayRTB.Document = DataHelpers.RTBDocuGen(SettingsManager.getDlcFolder());
            }

        }

        private void SetDownloadDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) //This shows the dialog, then if the result was 'ok' then it will do the following.
            {

                //Settings_Manager.settings[0] = dialog.SelectedPath;
                SettingsManager.settings[0] = dialog.SelectedPath;
                DataHelpers.populateDlcFilesList(SettingsManager.getDlcFolder(), SettingsManager.getCdlcFolder());
                DataHelpers.print("Download folder is now set to " + SettingsManager.settings[0], richTextBox);
                SettingsManager.requestWriteSettings(TransferButton, richTextBox);
                DownloadFolderDisplayRTB.Document = DataHelpers.RTBDocuGen(SettingsManager.settings[0]);
                //downFolder = dialog.SelectedPath;
                //statusBox.AppendText("Download folder has been set to " + downFolder);
            }
        }

        private void CleanupContentButton_Click(object sender, RoutedEventArgs e)
        {
            FileHandling.ReqCleanup(richTextBox);
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            auto = auto ? false : true;
            setLabel();
            if (auto)
            {
                DoAuto();
            }
            else
            {
                stopAuto();
            }
        }
        public void DoAuto()
        {
            if (SettingsManager.settings[0] != "")
            {
                FS.Path = SettingsManager.settings[0];
                FS.IncludeSubdirectories = true;
                FS.Filter = "*.psarc";
                FS.Created += new FileSystemEventHandler(autoFile);
                FS.EnableRaisingEvents = true;
            }
        }
        public void stopAuto()
        {
            FS.EnableRaisingEvents=false;
        }
        public void autoFile(object source, FileSystemEventArgs e)
        {
            if (Dispatcher.CheckAccess())
            {
                Helpers.DataHelpers.print("New Song Detected: " + e.Name, richTextBox);
                FileHandling.ReqTransfer(richTextBox, SaveOrginalCB, AutoSortCB, AutoBackupCB);
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    Helpers.DataHelpers.print("New Song Detected: " + e.Name, richTextBox);
                    FileHandling.ReqTransfer(richTextBox, SaveOrginalCB, AutoSortCB, AutoBackupCB);
                });
            }
        }
        //public void autoFile(object source, FileSystemEventArgs e)
        //{
        //    if (InvokeRequired)
        //    {
        //        BeginInvoke(new Action(() => { Helpers.DataHelpers.print("New Song Detected: " + e.Name, richTextBox); }));
        //        FileHandling.ReqTransfer(richTextBox, SaveOrginalCB, AutoSortCB, AutoBackupCB);
        //    }

        //}
        private void setLabel()
        {
            Brush green = new SolidColorBrush(Colors.Green);
            Brush red = new SolidColorBrush((Colors.DarkRed));
            //Brush red = new SolidBrush(Color.DarkRed);
            Autolabel.Background = auto ? green : red;
        }

        private void SaveOrginalCB_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.setSaveOriginal(SaveOrginalCB);
            SettingsManager.requestWriteSettings(TransferButton, richTextBox);
        }

        private void AutoSortCB_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.setOrganize(AutoSortCB);
            SettingsManager.requestWriteSettings(TransferButton, richTextBox);
        }
    }
}
