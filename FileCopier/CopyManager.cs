using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    /// <summary>
    /// it is this class that responds to the gui level interactions,
    /// whenever there is a request to copy some data, it enters the destination and the source in a file that is stored in the temp directory,
    /// then informs the file copier to being processing the data.    
    /// </summary>
    class CopyManager : IDisposable
    {
        private FileCopier _fileCopier = new FileCopier();
        private FileSystemWatcher _fileWatcher = new FileSystemWatcher();

        public static string TempFilePath { get; } = @"C:\Users\Vishesh\AppData\Local\Temp\queuedCopier.txt";

        public CopyManager()
        {
            if(!Directory.Exists(Path.GetDirectoryName(TempFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TempFilePath));                
            }

            _fileWatcher.Path = Path.GetDirectoryName(TempFilePath);
            _fileWatcher.Filter = Path.GetFileName(TempFilePath);
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileWatcher.Changed += OnFileChanged;

            _fileWatcher.EnableRaisingEvents = true;        
        }

        public void Dispose()
        {
            File.Delete(TempFilePath);
        }

        private void StartCopying()
        {
            if (!_fileCopier.Copying)
            {
                _fileCopier.CopyData();
            }
        }

        protected virtual void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            CopyData data = new CopyData();
            JsonHandler.GetDataFromFile(TempFilePath, out data);
            _fileCopier.RegisterDataToCopy(data);
            StartCopying();
        }

        public void WaitTillBatchCopied()
        {
            while (_fileCopier.Copying) ;
        }
    }

    class CopyManagerTestVersion : CopyManager
    {
        public bool FileModifiedEventRaised { get; private set; } = false;
        public CopyData CopyData = new CopyData();

        protected override void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            base.OnFileChanged(sender, e);
            JsonHandler.GetDataFromFile(TempFilePath, out CopyData);
            FileModifiedEventRaised = true;
        }
    }

    [TestFixture]
    class TestCopyManager
    {
        [Test]
        public void test_onFileChangedEvent()
        {
            using (var copyManagerTestVersion = new CopyManagerTestVersion())
            {
                var text = File.AppendText(CopyManager.TempFilePath);
                var jsonString = @"{""DestinationDirectory"" : ""F:\\RoughWork\\destination"",""Entity"" : [""F:\\RoughWork\\source\\DSC00323.JPG"", ""F:\\RoughWork\\source\\Raging.Bull.1980.720p.BluRay.x264.YIFY.srt""]}";
                text.Write(jsonString);
                text.Close();

                while (!copyManagerTestVersion.FileModifiedEventRaised) ;
                Assert.AreEqual(true, copyManagerTestVersion.FileModifiedEventRaised);
                Assert.IsTrue(copyManagerTestVersion.CopyData.Entity.Count == 2);
            }        
        }

        [Test]
        public void test_copy_files()
        {
            var copyManagerTestVersion = new CopyManagerTestVersion();
            var jsonString = @"{""DestinationDirectory"" : ""F:\\RoughWork\\destination"",""Entity"" : [""F:\\RoughWork\\source\\Multiples 3,5.js"", ""F:\\RoughWork\\source\\output.html""]}";
            File.WriteAllText(CopyManager.TempFilePath, jsonString);

            Thread.Sleep(30);
            jsonString = @"{""DestinationDirectory"" : ""F:\\RoughWork\\destination"",""Entity"" : [""F:\\RoughWork\\source\\icon.ico"", ""F:\\RoughWork\\source\\layout.css""]}";
            File.WriteAllText(CopyManager.TempFilePath, jsonString);

            while (!copyManagerTestVersion.FileModifiedEventRaised) ;
            copyManagerTestVersion.WaitTillBatchCopied();
            Assert.IsTrue(File.Exists(@"F:\RoughWork\destination\Multiples 3,5.js"));
            Assert.IsTrue(File.Exists(@"F:\RoughWork\destination\output.html"));
            Assert.IsTrue(File.Exists(@"F:\RoughWork\source\icon.ico"));
            Assert.IsTrue(File.Exists(@"F:\RoughWork\source\layout.css"));
        }

    }
}
