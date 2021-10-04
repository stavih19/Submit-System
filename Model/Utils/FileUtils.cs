using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Diagnostics;
using System.Security;
using System;
using System.Linq;
using System.Threading;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace Submit_System
{
    /// <summary>
    ///     Functions for working with files.
    /// </summary>
    public static class FileUtils
    {
        const int MAX_LENGTH = 1024*1024 * 10;

        private static readonly Random _random;
        public static readonly Dictionary<string, string[]> LangToExts;
        private static readonly string[] codeExts = {".py", ".cs", ".cpp", ".c", ".cc", ".asm"};
        static FileUtils()
        {
            _random = new Random();
            LangToExts = new Dictionary<string, string[]>
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
        }
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
        public static IEnumerable<string> EnumerateFiles(string folder, string[] exts)
        {
            var files = Enumerable.Empty<string>();
            foreach (string ext in exts)
            {
                var newFiles = Directory.EnumerateFiles(folder, ext, SearchOption.AllDirectories);
                files = files.Concat(newFiles);
            }
            return files;
        }
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
            string relPath = GetRelativePath(file, folder).Replace("/", "_-_").Replace("\\", "_-_");
            return folder + '/' + relPath;
        }
        /// <summary>
        /// Combines Paths like Path.Combine, but throws an excpetion if the resulting file path is
        /// is not within the first directory
        /// </summary>
        /// <param name="filename"> the name/relative path of the file from the directory</param>
        /// <param name="directory">The path of the directory</param>
        /// <returns></returns>
        public static string PathSafeCombine(string directory, string filename)
        {
            var fullDirPath = Path.GetFullPath(directory);
            var filePath = Path.Combine(directory, filename);
            var fullFilePath = Path.GetFullPath(filePath);
            if(!fullFilePath.StartsWith(fullDirPath))
            {
                throw new SecurityException("Attempt to access outside of the allowed folder");
            }
            return filePath;
        }
        private static List<string> DecompressAndStore(byte[] archive, string destDir)
        {   
            var files = new List<string>();
            using (Stream stream = new MemoryStream(archive))
            {
                using (ZipArchive zip = new ZipArchive(stream))
                {
                    foreach(var entry in zip.Entries)
                    {
                        string newPath = PathSafeCombine(destDir, entry.FullName);
                        entry.ExtractToFile(newPath, true);
                        files.Add(newPath);
                    }
                }
            }
            return files;
        }
        public static List<string> StoreFiles(List<SubmitFile> files, string destDir, bool deletePreviousFiles, bool unzip)
        {
            var submittedFiles = new List<string>();
            if(!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            try
            {
                foreach(var file in  files)
                {
                    int pos = file.Content.IndexOf(',');
                    file.Content = file.Content.Substring(pos+1);
                    byte[] content = System.Convert.FromBase64String(file.Content);
                    if(Path.GetExtension(file.Name) == ".zip" && unzip)
                    {
                        submittedFiles.AddRange(DecompressAndStore(content, destDir));
                    }
                    else
                    {
                        string destPath = PathSafeCombine(destDir, file.Name);
                        File.WriteAllBytes(destPath, content);
                        submittedFiles.Add(destPath);
                    }
                }
                if(!deletePreviousFiles)
                {
                    return GetRelativePaths(destDir, submittedFiles);
                }
                var a = Directory.EnumerateFiles(destDir, "*", SearchOption.AllDirectories);
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
            catch (Exception e) when (deletePreviousFiles)
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
        public static byte[] ToArchive(string path)
        {
            byte[] result;
            if(!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException();
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
        public static string GetMimeType(string filename)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            bool success = provider.TryGetContentType(filename, out contentType);
            if(success)
            {
               return contentType;
            }
            else if(codeExts.Contains(Path.GetExtension(filename)))
            {
                return "text/plain";
            }
            else
            {
                return "application/octet-stream";
            }
        }
        /// <summary>
        /// Creates a directory with a unique
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string CreateUniqueDirectory(string dir, string prefix)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string path;
            var builder = new StringBuilder();
            lock(_random)
            {
                do
                {
                    for (int i = 0; i < 6; i++)
                    {
                        builder.Append(chars[_random.Next(chars.Length)]);
                    }
                    string result = prefix + '_' + builder.ToString();
                    path = Path.Combine(dir, result);
                    builder.Clear();
                } while (Directory.Exists(path));
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public static void CopyDirectory(string src, string dist)
        {
            DirectoryInfo dir = new DirectoryInfo(src);

            DirectoryInfo[] dirs = dir.GetDirectories();
            
            Directory.CreateDirectory(dist);        

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(dist, file.Name);
                file.CopyTo(tempPath, true);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(dist, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }
        public static string GetTempFileName(string ext)
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString() + ext;
        }
        public static void DeleteDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            string[] dirs = Directory.GetDirectories(directory);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            Directory.Delete(directory, false);
        }
    }
}
