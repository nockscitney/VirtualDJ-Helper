using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//  Added
using System.Xml;
//  Newtonsoft
using Newtonsoft.Json;
//  Project
using NickScotney.Internal.VDJ.LogicLibrary.Objects;

namespace NickScotney.Internal.VDJ.LogicLibrary.Controllers
{
    public class HistoryController
    {
        public static List<History> ReadHistory(string fileName)
        {
            History history = null;
            List<History> historyList = null;
            string[] fileContents = null;

            if ((fileContents = System.IO.File.ReadAllLines(fileName)) != null)
            {
                //  For each file line in the file
                foreach (string fileLine in fileContents)
                {
                    //  If the line starts with #EXTVDJ: (i.e. it's a new track listing)
                    if (fileLine.Substring(0, 8).ToLower() == "#extvdj:")
                    {
                        //  Create a new XML Document here
                        XmlDocument doc = new XmlDocument();

                        //  Check to see if history list is null, and initialize it if it is
                        if (historyList == null)
                            historyList = new List<History>();

                        //  Add the history item to the list
                        historyList.Add(history);

                        //  Clear the contents of history here
                        history = null;

                        //  set the XML document to the current line, converting any & to &amp;
                        doc.LoadXml(String.Format("<root>{0}</root>", fileLine.Substring(8).Replace("&", "&amp;")));

                        //  Convert the XML to JSON, and then deserialize to a History object
                        history = JsonConvert.DeserializeObject<History>(JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true));
                    }
                    //  Else, we're dealing with the track location, so add this to history
                    else
                        history.TrackLocation = fileLine;
                }
            }

            //  Finally, check to see if history is null, and if not add it to the list
            if (history != null)
                historyList.Add(history);

            return historyList;
        }
    }
}
