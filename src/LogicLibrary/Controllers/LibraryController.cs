using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//  Added
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
//  Newtonsoft
using Newtonsoft.Json;
//  Project
using NickScotney.Internal.VDJ.LogicLibrary.Objects;

namespace NickScotney.Internal.VDJ.LogicLibrary.Controllers
{
    public class LibraryController
    {
        public static ObservableCollection<LibraryItem> ReadLibrary(string fileName)
        {
            ObservableCollection<LibraryItem> libraryList = null;

            if (File.Exists(fileName))
            {
                XDocument input = XDocument.Load(fileName);
                List<XElement> children = input.Root.Elements().ToList();

                foreach (XElement child in children)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(child.ToString());

                    if (libraryList == null)
                        libraryList = new ObservableCollection<LibraryItem>();

                    string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None);
                    var jsonObject = JsonConvert.DeserializeObject(json);

                    LibraryItem song = JsonConvert.DeserializeObject<LibraryItem>(json);

                    libraryList.Add(JsonConvert.DeserializeObject<LibraryItem>(json));
                }
            }

            return libraryList;
        }
    }
}
