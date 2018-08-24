﻿using System;
using System.Collections.Generic;
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

//  Added
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Windows.Threading;
using System.Xml;
//  Logic
using NickScotney.Internal.VDJ.LogicLibrary.Controllers;
using NickScotney.Internal.VDJ.LogicLibrary.Objects;
//  Newtonsoft
using Newtonsoft.Json;

namespace NickScotney.Internal.VDJ.VDJ_Helper
{
    delegate void CallBackToUIThread(FileSystemEventArgs e);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer clockTimer;
        List<DriveInfo> drives;

        public ObservableCollection<LibraryItem> LibraryList = null;
        public string CurrentTrack
        {
            set { lblCurrentTrack.Content = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            drives = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  Start the clock timer here
            ClockStart();

            //  Start the history monitor here
            HistoryMonitor();

            //  Scan for active VDJ Drives
            ScanDrives();
        }


        private void Button_Click(object sender, RoutedEventArgs e) => LoadTracks();

        private void ClockTimer_Tick(object sender, EventArgs e) => lblCurrentTime.Content = String.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        private void cmbBxLibraries_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadTracks();


        public void NotifyUIThreadOfChange(FileSystemEventArgs e)
        {
            string lastLine = String.Empty;

            if (IsFileClosed(e.FullPath, true))
            {
                lastLine = File.ReadAllLines(e.FullPath).Last();

                if (lastLine.Substring(2, 1) != ":")
                    return;

                CurrentTrack = lastLine.Substring(8);
            }
        }


        private void fsw_Changed(object sender, FileSystemEventArgs e) => this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new CallBackToUIThread(NotifyUIThreadOfChange), e);



        void ClockStart()
        {
            clockTimer = new DispatcherTimer();
            clockTimer.Tick += new EventHandler(ClockTimer_Tick);
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Start();
        }

        void HistoryMonitor()
        {
            FileSystemWatcher fsw = new FileSystemWatcher();
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"VirtualDJ\History");

            fsw.Path = path;

            fsw.EnableRaisingEvents = true;
            fsw.Changed += fsw_Changed;

        }

        //  Method taken from https://stackoverflow.com/questions/21739242/filestream-and-a-filesystemwatcher-in-c-weird-issue-process-cannot-access-the
        bool IsFileClosed(string filepath, bool wait)
        {
            bool fileClosed = false;
            int retries = 20;
            const int delay = 500; // Max time spent here = retries*delay milliseconds

            if (!File.Exists(filepath))
                return false;

            do
            {
                try
                {
                    // Attempts to open then close the file in RW mode, denying other users to place any locks.
                    FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    fs.Close();
                    fileClosed = true; // success
                }
                catch (IOException) { }

                if (!wait) break;

                retries--;

                if (!fileClosed)
                    System.Threading.Thread.Sleep(delay);
            }
            while (!fileClosed && retries > 0);

            return fileClosed;
        }

        void LoadTracks()
        {
            if (cmbBxLibraries.SelectedIndex > 0)
            {
                ListCollectionView collectionView = new ListCollectionView(LibraryController.ReadLibrary(String.Concat(drives[cmbBxLibraries.SelectedIndex - 1].RootDirectory.Root, @"VirtualDJ\database.xml")));
                collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Song.FolderPath"));
                collectionView.SortDescriptions.Add(new SortDescription("Song.FolderPath", ListSortDirection.Ascending));
                dgTrackList.ItemsSource = collectionView;
            }
        }

        void ScanDrives()
        {
            //  Clear the combo box
            cmbBxLibraries.Items.Clear();

            //  Clear the drives list if it's not null
            if(drives != null)
                drives.Clear();
            
            //  Set a default option here
            cmbBxLibraries.Items.Add("Select Library");
            cmbBxLibraries.SelectedIndex = 0;

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (File.Exists(String.Concat(drive.RootDirectory.Root, @"VirtualDJ\database.xml")))
                {
                    //  Add the drive to the combo box
                    cmbBxLibraries.Items.Add(String.Concat("Library on ", drive.RootDirectory.Root));

                    //  If the drives list is null, initialize it here
                    if (drives == null)
                        drives = new List<DriveInfo>();

                    //  Add the drive to the list here
                    drives.Add(drive);
                }
            }
            
        }
    }
}