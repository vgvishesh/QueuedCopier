using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopier
{
    /// <summary>
    /// this data sructure holds the data that has to be copied,
    /// a destination directory, and list of all the files and the directories that have to copied.
    /// 
    /// /// the data format of the file having the copy information is json:
    /// { DestinationDirectory : destination_value
    ///   Entity : [ file1,
    ///                     dir1,
    ///                     ... ]
    /// }
    /// </summary>
    public class CopyData
    {
        public string DestinationDirectory { get; set; }
        public List<string> Entity = new List<string>();

        public CopyData()
        { }

        public CopyData(string destination, params string[] files)
        {
            DestinationDirectory = destination;
            Entity.AddRange(files);
        }
    }
}
