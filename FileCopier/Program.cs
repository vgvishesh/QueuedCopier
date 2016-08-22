using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileCopier
{
    /// <summary>
    /// this data sructure holds the data that has to be copied,
    /// a destination directory, and list of all the files and the directories that have to copied.
    /// </summary>
    public class CopyData
    {
        public string DestinationDirectory { get; set; }
        public List<string> Entity = new List<string>();       
        
        public CopyData(string destination, params string[] files)
        {
            DestinationDirectory = destination;
            Entity.AddRange(files);
        }
    }

    public class FileCopier
    {
        private List<CopyData> _data = new List<CopyData>();

        internal void InitiateCopy(IEnumerable<CopyData> copyData)
        {
            _data.AddRange(copyData);
        }

        internal void CompleteDataCopy()
        {
            _data.ForEach(data =>
            {
                data.Entity.ForEach(entity =>
                {
                    if((File.GetAttributes(entity) & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        CopyDirectory(entity, data.DestinationDirectory);
                    }
                    else
                    {
                        CopyFile(entity, data.DestinationDirectory);
                    }
                });
            });
        }
        
        internal void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            var sourceDirectoryName = Path.GetFileName(sourceDirectory);
            foreach(var file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                var destinationPath = Path.Combine(destinationDirectory, file.Substring(file.IndexOf(sourceDirectoryName)));
                var fileDirectory = Path.GetDirectoryName(destinationPath);

                if(!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                CopyFile(file, fileDirectory);
            }
        }

        internal void CopyFile(string sourceFile, string destinationDirectory)
        {
            var destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(sourceFile));
            File.Copy(sourceFile, destinationPath, true);
        }
    }

    [TestFixture]
    public class FileCopierTest
    {
        [Test]
        public void test_single_file_copy()
        {
            var fileCopier = new FileCopier();
            var sourceFile = @"F:\RoughWork\source\DSC00323.JPG";
            var destination = @"F:\RoughWork\destination";

            fileCopier.CopyFile(sourceFile, destination);
            Assert.AreEqual(true, File.Exists(Path.Combine(destination, Path.GetFileName(sourceFile))));
        }

        [Test]
        public void test_directory_copy()
        {
            var fileCopier = new FileCopier();
            var sourceFile = @"F:\RoughWork\source";
            var destination = @"F:\RoughWork\destination";

            fileCopier.CopyDirectory(sourceFile, destination);
            Assert.AreEqual(true, Directory.Exists(destination));
        }

        [Test]
        public void test_copy_mulitple_items()
        {
            var fileCopier = new FileCopier();
            var data = new List<CopyData>
            {
                new CopyData(@"F:\RoughWork\destination", @"F:\RoughWork\source\WHAT", @"F:\RoughWork\source\Raging.Bull.1980.720p.BluRay.x264.YIFY.srt"),
                new CopyData(@"F:\RoughWork\destination", @"F:\Movies\Good Will Hunting (1997)")
            };

            fileCopier.InitiateCopy(data);
            fileCopier.CompleteDataCopy();

            Assert.AreEqual(true, Directory.Exists(@"F:\RoughWork\destination\Good Will Hunting (1997)"));
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
