using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Submit_System
{
    /// <summary>
    ///     Functions for working with files.
    /// </summary>
    public static class FileUtils
    {
        public static readonly Dictionary<string, string[]> LangToExt = new Dictionary<string, string[]>
        {
            ["c"] = new string[2] { "*.c", "*.h" },
            ["cc"] = new string[3] { "*.cpp", "*.h", "*.cc" },
            ["ml"] = new string[1] { "*.ml" },
            ["java"] = new string[1] { "*.java" },
            ["csharp"] = new string[1] { "*.cs" },
            ["python"] = new string[1] { "*.py" },
            ["javascript"] = new string[1] { "*.js" },
            ["perl"] = new string[1] { "*.pl" },
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
                var newFiles = Directory.GetFiles(folder, ext, System.IO.SearchOption.AllDirectories);
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

        public static void StoreFiles(string path, List<IFormFile> files)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach(var file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(path, file.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }  
        }
    }
}
