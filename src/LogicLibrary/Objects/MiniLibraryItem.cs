using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
