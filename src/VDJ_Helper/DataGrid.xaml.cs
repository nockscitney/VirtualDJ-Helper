using System;
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
using System.Windows.Shapes;

//  Added
using System.Collections.ObjectModel;
//  Logic
using NickScotney.Internal.VDJ.LogicLibrary.Controllers;
using NickScotney.Internal.VDJ.LogicLibrary.Objects;

namespace NickScotney.Internal.VDJ.VDJ_Helper
{
    /// <summary>
    /// Interaction logic for DataGrid.xaml
    /// </summary>
    public partial class DataGrid : Window
    {
        ObservableCollection<MainLibraryItem> myLibrary;

        public DataGrid()
        {
            InitializeComponent();

            myLibrary = new ObservableCollection<MainLibraryItem>();

           GetLibraryItems();
        }


        void GetLibraryItems()
        {
            myLibrary.Add(new MainLibraryItem { Item = new MiniLibraryItem { Comment = "Hi There", Group = "One" } });
            myLibrary.Add(new MainLibraryItem { Item = new MiniLibraryItem { Comment = "Hi There", Group = "One" } });
            myLibrary.Add(new MainLibraryItem { Item = new MiniLibraryItem { Comment = "Hi There", Group = "One" } });
            myLibrary.Add(new MainLibraryItem { Item = new MiniLibraryItem { Comment = "Hi There", Group = "Two" } });
            myLibrary.Add(new MainLibraryItem { Item = new MiniLibraryItem { Comment = "Hi There", Group = "Two" } });

            ListCollectionView collectionView = new ListCollectionView(myLibrary);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Item.Group"));
            dgTest.ItemsSource = collectionView;
        }

        //List<LibraryItem> GetLibraryItems()
        //{
        //    List<LibraryItem> libraryItems = LibraryController.ReadLibrary(@"G:\VirtualDJ\database.xml");// new List<LibraryItem>();

        //    return libraryItems;
        //}
    }
}