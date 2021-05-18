using System.Collections.Generic;
using System.IO;
namespace Submit_System
{
    public class SubmitFile
    {
        public string filename {get; set;}
        public string path {get; set;}
        public List<SubmitFile> SubFiles {get; set;}
        public static SubmitFile Create(string filePath)
        {
            var submitFile = new SubmitFile
            {
                filename = FileUtils.GetFileName(filePath),
                path = filePath,
                SubFiles = new List<SubmitFile>()
            };
            string[] files;
            bool isDir = (File.GetAttributes(filePath) & FileAttributes.Directory)
                 == FileAttributes.Directory;
            if(!isDir)
            {
                return submitFile;
            }
            try
            {
                files = Directory.GetFiles(filePath);
            }
            catch
            {
                return submitFile;
            }
            foreach(var file in files)
            {
                submitFile.SubFiles.Add(Create(file));
            }
            return submitFile;
        }

    }
}