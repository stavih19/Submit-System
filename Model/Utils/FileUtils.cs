using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Diagnostics;
using System.Security;
using System;

namespace Submit_System
{
    /// <summary>
    ///     Functions for working with files.
    /// </summary>
    public static class FileUtils
    {
        const int MAX_LENGTH = 1024*1024 * 10;
        public static readonly Dictionary<string, string[]> LangToExts = new Dictionary<string, string[]>
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
        public static string[] GetExts(string language)
        {
            return LangToExts[language];
        }
        public static bool ContainsLang(string language)
        {
            return LangToExts.ContainsKey(language);
        }
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
            string relPath = GetRelativePath(file, folder).Replace("/", "_-_");
            return folder + '/' + relPath;
        }

        /// <summary>
        /// Checks if the file has a valid full path with respect to its directory
        /// </summary>
        /// <param name="filename"> the name/relative path of the file from the directory</param>
        /// <param name="dirPath">The path of the directory</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFullPath(string filename, string dirPath)
        {
            var fullDirPath = Path.GetFullPath(dirPath);
            var filePath = Path.Combine(dirPath, filename);
            var fullFilePath = Path.GetFullPath(filePath);
            if(!fullFilePath.StartsWith(fullDirPath))
            {
                throw new SecurityException("Attempt to access outside of the allowed folder");
            }
            return filePath;
        }
        private static void CopyToFile(Stream stream, string destPath)
        {
            using(Stream destStream = File.OpenWrite(destPath))
            {
                stream.CopyTo(destStream);
            }
        }
        private static List<string> DecompressAndStore(Stream stream, string destDir)
        {   
            var files = new List<string>();
            using (ZipArchive zip = new ZipArchive(stream))
            {
                foreach(var entry in zip.Entries)
                {
                    string newPath = GetFullPath(entry.FullName, destDir);
                    entry.ExtractToFile(newPath, true);
                    files.Add(newPath);
                }
            }
            return files;
        }
        public static List<string> SubmitFiles(List<SubmitFile> files, string destDir)
        {
            var submittedFiles = new List<string>();
            if(!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            try
            {
                foreach(var file in files)
                {
                    byte[] content = System.Convert.FromBase64String(file.Content);
                    using(Stream stream = new MemoryStream(content))
                    {
                        if(Path.GetExtension(file.Name) == ".zip")
                        {
                            submittedFiles.AddRange(DecompressAndStore(stream, destDir));
                        }
                        else
                        {
                            string destPath = GetFullPath(file.Name, destDir);
                            CopyToFile(stream, destPath);
                            submittedFiles.Add(destPath);
                        }
                    }
                }
                var a = Directory.GetFiles(destDir, "*", SearchOption.AllDirectories);
                foreach(var file in a )
                {
                    if(!submittedFiles.Contains(file))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch {}
                    }
                }
            }
            catch (Exception e)
            {
                Directory.Delete(destDir);
                throw e;
            }
            return GetRelativePaths(destDir, submittedFiles);
        }
        /// <summary>
        /// Returns the path of the file "filePath" relative to the directory "dirPath" 
        /// </summary>
        /// <param name="file">the file</param>
        /// <param name="dir">an ancestor directory</param>
        /// <returns>filePath's relaative to </returns>
        public static string GetRelativePath(string file, string dir)
        {
            var newFileName = file.Replace(dir, "");
            if(newFileName.StartsWith('/') || newFileName.StartsWith('\\'))
            {
                newFileName = newFileName.Remove(0, 1);
            }
            return newFileName;
        }
        public static List<string> GetRelativePaths(string folder)
        {
            try
            {
                if(Directory.Exists(folder))
                {
                    var files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
                    return GetRelativePaths(folder, files);
                }
            }
            catch {}
            return new List<string>();
        }
        public static List<string> GetRelativePaths(string folder, IEnumerable<string> files)
        {
            var fileList = new List<string>();
            foreach(var file in files)
            {
                fileList.Add(GetRelativePath(file, folder));
            }
            return fileList;
        }
        public static byte[] ToArchiveBytes(string path)
        {
            byte[] result;
            if(!Directory.Exists(path))
            {
                return null;
            }
            using(var stream = new MemoryStream())
            {
                using(var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
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
        public static void StoreFiles(List<IFormFile> files, string destDir)
        {
            if(!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            foreach(var file in files)
            {
                using(Stream stream = file.OpenReadStream())
                {
                    if(Path.GetExtension(file.FileName) == ".zip")
                    {
                        DecompressAndStore(stream, destDir);
                    }
                    else
                    {
                        string destPath = GetFullPath(file.FileName, destDir);
                        CopyToFile(stream, destPath);
                    }   
                }           
            }  
        }
    }
}
