using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopier
{
    class JsonHandler
    {
        public static void GetDataFromFile<T>(string filePath, out T data)
        {
            FileStream fileStream;
            string fileData;
            byte[] bytes;

            using (fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, (int)fileStream.Length);
            }

            fileData = Encoding.Default.GetString(bytes);            
            data = JsonConvert.DeserializeObject<T>(fileData);
        }
    }
}
