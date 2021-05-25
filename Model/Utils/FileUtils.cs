using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Diagnostics;

namespace Submit_System
{
    /// <summary>
    ///     Functions for working with files.
    /// </summary>
    public static class FileUtils
    {
        const int MAX_LENGTH = 1024*1024 * 10;
        public static readonly Dictionary<string, string[]> LangToExt = new Dictionary<string, string[]>
        {
            ["c"] = new string[] { "*.c", "*.h" },
            ["cc"] = new string[] { "*.cpp", "*.h", "*.cc" },
            ["ml"] = new string[] { "*.ml" },
            ["java"] = new string[] { "*.java" },
            ["csharp"] = new string[] { "*.cs" },
            ["python"] = new string[] { "*.py" },
            ["javascript"] = new string[] { "*.js" },
            ["perl"] = new string[] { "*.pl" },
        };
        /// <summary>
        ///     Gets all files from the folder with the given extensions
        /// </summary>
        /// <param name="folder">the folder to get the files from</param>
        /// <param name="exts">the file extensions</param>
        /// <returns>All the files in the given folder with the given extensions</returns>
        public static List<string> GetFiles(string folder, string[] exts)
        {
            var files = new List<string>();
            foreach (string ext in exts)
            {
                var newFiles = Directory.GetFiles(folder, ext, SearchOption.AllDirectories);
                files.AddRange(newFiles);
            }
            return files;
        }
        /// <summary>
        ///     Returns the last slash in a file path (unless it's the last character. Then it returns the previous one).
        /// </summary>
        /// <param name="path">the file path</param>
        /// <returns>The position of the last slash in the file path</returns>
        public static int LastSlash(string path)
        {
            if (path.EndsWith('/') || path.EndsWith('\\'))
            {
                path = path.Remove(path.Length - 1);
            }
            int linuxSlash = path.LastIndexOf('/');
            if (linuxSlash == -1)
            {
                return path.LastIndexOf('\\');
            }
            return linuxSlash;
        }
        /// <summary>
        ///     Returns the name of the file in the path.
        /// </summary>
        /// <param name="path">file path.</param>
        /// <returns>file name</returns>
        public static string GetFileName(string path)
        {
            return path[(LastSlash(path) + 1)..];
        }
        /// <summary>
        ///     Gives a fake file path that presents the file as a child of the folder
        /// </summary>
        /// <param name="folder">the name of a folde</param>
        /// <param name="file">the file path</param>
        /// <returns>The file path relative to the folder</returns>
        public static string FlattenFilePath(string folder, string file)
        {
            return folder + '/' + GetFileName(file);
        }

        public static bool TryGetValidPath(string filename, string destPath, out string filePath)
        {
            var fullDirPath = Path.GetFullPath(destPath);
            filePath = Path.Combine(destPath, filename);
            var fullFilePath = Path.GetFullPath(filePath);
            if(!fullFilePath.StartsWith(fullDirPath))
            {
                return false;
            }
            return true;
        }
        private static void CopyToFile(Stream stream, string destPath, string filename)
        {
            string newPath;
            if(!TryGetValidPath(filename, destPath, out newPath))
            {
                throw new System.ArgumentException("Attempt to access outside of the allowed folder");
            }
            using(Stream destStream = File.OpenWrite(newPath))
            {
                stream.CopyTo(destStream);
            }
        }
        private static void DecompressAndStore(Stream stream, string destPath)
        {   
            using (ZipArchive zip = new ZipArchive(stream))
            {
                foreach(var entry in zip.Entries)
                {
                    string newPath;
                    if(!TryGetValidPath(entry.FullName, destPath, out newPath))
                    {
                        throw new System.ArgumentException("Attempt to access outside of the allowed folder");
                    }
                    entry.ExtractToFile(newPath, true);    
                }
            }       
        }
        public static void StoreFiles(List<IFormFile> files, string destPath)
        {
            if(!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            foreach(var file in files)
            {
                using(Stream stream = file.OpenReadStream())
                {
                    if(Path.GetExtension(file.FileName) == ".zip")
                    {
                        DecompressAndStore(stream, destPath);
                    }
                    else
                    {
                        CopyToFile(stream, destPath, file.FileName);
                    }   
                }           
            }  
        }
        public static void StoreFiles(List<UploadedFile> files, string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach(var file in files)
            {
                byte[] content = System.Convert.FromBase64String(file.Content);
                using(Stream stream = new MemoryStream(content))
                {
                    if(Path.GetExtension(file.Name) == ".zip")
                    {
                        DecompressAndStore(stream, path);
                    }
                    else
                    {
                        CopyToFile(stream, path, file.Name);
                    }
                }
            }  
        }

        public static string GetRelativePath(string filePath, string dirPath)
        {
            var newFileName = filePath.Replace(dirPath, "");
            if(newFileName.StartsWith('/') || newFileName.StartsWith('\\'))
            {
                newFileName = newFileName.Remove(0, 1);
            }
            return newFileName;
        }
        public static List<string> GetRelativePaths(string folder)
        {
            var fileList = new List<string>();
            if(Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach(var file in files)
                {
                    fileList.Add(GetRelativePath(file, folder));
                }
            }
            return fileList;
        }
        public static byte[] ToArchiveBytes(string path)
        {
            byte[] result;
            using(var stream = new MemoryStream())
            {
                using(var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    foreach(var file in files)
                    {
                        var newFileName = GetRelativePath(file, path);
                        zip.CreateEntryFromFile(file, newFileName);
                    }
                }
                result = stream.ToArray();
            }
            return result;
        }
    }
}
