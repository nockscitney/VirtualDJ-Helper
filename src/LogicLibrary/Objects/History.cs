using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NickScotney.Internal.VDJ.LogicLibrary.Objects
{
    public class History
    {
        public int Filesize { get; set; }
        public string Artist { get; set; }
        public string Lastplaytime { get; set; }
        public string Remix { get; set; }
        public string Time { get; set; }
        public string TrackLocation { get; set; }
        public string Title { get; set; }

        public History() 
            : base()
        {
            Filesize = int.MinValue;
        }
    }
}
