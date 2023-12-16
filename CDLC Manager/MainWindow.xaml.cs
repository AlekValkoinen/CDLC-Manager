using CDLC_Manager.FileTools;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            Settings.Settings_Manager.checkForSettings(richTextBox, SaveOrginalCB, AutoSortCB, TransferButton, AutoBackupCB);
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

        }

        private void SetDownloadDirectoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CleanupContentButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
