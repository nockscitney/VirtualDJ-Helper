using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

//  Added
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;
//  Logic
using NickScotney.Internal.VDJ.LogicLibrary.Controllers;
using NickScotney.Internal.VDJ.LogicLibrary.Objects;

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
        ListCollectionView masterCollectionView;
        string currentTrack;

        public ObservableCollection<LibraryItem> LibraryList = null;
        public string CurrentTrack
        {
            get => currentTrack;
            set
            {
                currentTrack = value;
                lblCurrentTrack.Content = value;
            }
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

        private void btnRefreshLibrary_Click(object sender, RoutedEventArgs e) => LoadTracks(true);


        private void chkBxNewTracks_Checked(object sender, RoutedEventArgs e) => FilterUnPlayed();

        private void chkBxNewTracks_Unchecked(object sender, RoutedEventArgs e) => LoadTracks();

        private void ClockTimer_Tick(object sender, EventArgs e) => lblCurrentTime.Content = String.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        private void cmbBxLibraries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((bool)chkBxNewTracks.IsChecked)
            {
                chkBxNewTracks.Checked -= chkBxNewTracks_Checked;
                chkBxNewTracks.IsChecked = false;
                chkBxNewTracks.Checked += chkBxNewTracks_Checked;
            }

            chkBxNewTracks.IsEnabled = cmbBxLibraries.SelectedIndex > 0;
            LoadTracks(true);
        }

        private void fsw_Changed(object sender, FileSystemEventArgs e) => this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new CallBackToUIThread(NotifyUIThreadOfChange), e);

        public void NotifyUIThreadOfChange(FileSystemEventArgs e)
        {
            string lastLine = String.Empty;

            if (IsFileClosed(e.FullPath, true))
            {
                lastLine = File.ReadAllLines(e.FullPath).Last();

                if (lastLine.Substring(2, 1) != ":")
                    return;

                if (CurrentTrack != lastLine.Substring(8))
                {
                    CurrentTrack = lastLine.Substring(8);
                    txtBxSessionHistory.Text += String.IsNullOrEmpty(txtBxSessionHistory.Text) ? $"{DateTime.Now} : {currentTrack}" : $"{Environment.NewLine}{DateTime.Now} : {currentTrack}";
                }
            }
        }



        void ClockStart()
        {
            clockTimer = new DispatcherTimer();
            clockTimer.Tick += new EventHandler(ClockTimer_Tick);
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Start();
        }

        void FilterUnPlayed()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(dgTrackList.ItemsSource);

            cv.Filter = o =>
            {
                LibraryItem li = o as LibraryItem;
                return (li.Song.Infos.PlayCount == 0);
            };
        }

        //  Create the filewatcher, which will watch for changes in the VDJ History folder
        void HistoryMonitor()
        {
            FileSystemWatcher fsw = new FileSystemWatcher();
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"VirtualDJ\History");

            fsw.Path = path;

            fsw.EnableRaisingEvents = true;
            fsw.Changed += fsw_Changed;

        }

        //  Method taken from https://stackoverflow.com/questions/21739242/filestream-and-a-filesystemwatcher-in-c-weird-issue-process-cannot-access-the
        //  This will wait for the file to be closed before trying to read it, to prevent file opened exceptions
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

        //  Routine which will load the track data, and add them to the data grid, via a group
        void LoadTracks(bool newLibrary = false)
        {
            if (cmbBxLibraries.SelectedIndex > 0)
            {
                ListCollectionView collectionView;

                //  This will re-load the data from the library
                if (newLibrary)
                {
                    collectionView = new ListCollectionView(LibraryController.ReadLibrary(String.Concat(drives[cmbBxLibraries.SelectedIndex - 1].RootDirectory.Root, @"VirtualDJ\database.xml")));
                    masterCollectionView = collectionView;

                    collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Song.FolderPath"));
                    collectionView.SortDescriptions.Add(new SortDescription("Song.FolderPath", ListSortDirection.Ascending));
                }
                else
                    collectionView = masterCollectionView;

                dgTrackList.ItemsSource = collectionView;
                dgTrackList.Items.Refresh();

                if ((bool)chkBxNewTracks.IsChecked)
                    FilterUnPlayed();
            }
        }

        //  Routine which is used to scan each logical drive and check to see if there is a virtual dj folder in the root. Id there is, a library exists on the drive, so add it to the combo box
        void ScanDrives()
        {
            //  Clear the combo box
            cmbBxLibraries.Items.Clear();

            //  Clear the drives list if it's not null
            if (drives != null)
                drives.Clear();

            //  Set a default option here
            cmbBxLibraries.Items.Add("Select Library");
            //  Set the index of the combobox to 0
            cmbBxLibraries.SelectedIndex = 0;

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                //  Check to see if a database exists on the drive
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
