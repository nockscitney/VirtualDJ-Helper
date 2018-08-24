///
/// THIS IS A TEST CLASS
/// 
/// This class was only creaeted to test out nested classes within the datagrid binding and should therefore be ignored
///
namespace NickScotney.Internal.VDJ.LogicLibrary.Objects
{
    public class MiniLibraryItem
    {
        public string Comment { get; set; }
        public string Group { get; set; }

        public MiniLibraryItem()
            : base()
        { }
    }

    public class MainLibraryItem
    {
        public MiniLibraryItem Item { get; set; }

        public MainLibraryItem()
            : base()
        { }
    }
}
