using NUnit.Framework;
using System.IO;

/// <summary>
/// data that we needed fron the file copied to display in the gui
/// 1. the present directory and then file being copied
/// 2. the progress on present directory if files belonging to a directory are being copied, other wise the file
/// 3. the list of pending files/directories to be copied
/// 
/// /// the data format of the file having the copy information is json:
/// { DestinationDirectory : destination_value
///   Entity : [ file1,
///                     dir1,
///                     ... ]
/// </summary>

namespace FileCopier
{
    class CopyManager
    {
        private FileCopier _fileCopier = new FileCopier();

        public CopyManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyData"> this is a json string, having the information of the files and destination</param>
        public void CopyData(string copyData)
        {
            CopyData data = new CopyData();
            JsonHandler.GetDataFromString(copyData, out data);
            _fileCopier.RegisterDataToCopy(data);
            StartCopying();
        }

        private void StartCopying()
        {
            if(!_fileCopier.Copying)
            {
                _fileCopier.CopyData();
            }
        }

        public void WaitTillBatchCopied()
        {
            while (_fileCopier.Copying);
        }

    }

    [TestFixture]
    class TestCopyManager
    {
        [Test]
        public void test_copy_files()
        {
            var copyManagerTestVersion = new CopyManager();
            var jsonString = @"{""DestinationDirectory"" : ""F:\\RoughWork\\destination"",""Entity"" : [""F:\\RoughWork\\source\\Multiples 3,5.js"", ""F:\\RoughWork\\source\\output.html""]}";
            copyManagerTestVersion.CopyData(jsonString);

            jsonString = @"{""DestinationDirectory"" : ""F:\\RoughWork\\destination"",""Entity"" : [""F:\\RoughWork\\source\\icon.ico"", ""F:\\RoughWork\\source\\layout.css""]}";
            copyManagerTestVersion.CopyData(jsonString);

            copyManagerTestVersion.WaitTillBatchCopied();
            Assert.IsTrue(File.Exists(@"F:\RoughWork\destination\Multiples 3,5.js"));
            Assert.IsTrue(File.Exists(@"F:\RoughWork\destination\output.html"));
            Assert.IsTrue(File.Exists(@"F:\RoughWork\source\icon.ico"));
            Assert.IsTrue(File.Exists(@"F:\RoughWork\source\layout.css"));
        }
    }
}