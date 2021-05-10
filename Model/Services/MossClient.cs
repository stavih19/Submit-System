using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Submit_System
{

    /// <summary>
    ///     <c>MossClient</c> - Client for communicating with the MOSS server socket. Handles the
    ///     configuration, sending of files and receiving results.
    /// </summary>
    public class MossClient
    {
         public static readonly List<string> SupportedLangauges = new List<string> {
            "c", "cc", "java", "ml", "pascal", "ada", "lisp", "schema", "haskell",
            "fortran", "ascii", "vhdl", "perl","matlab", "python", "mips", "prolog",
            "spice", "vb", "csharp", "modula2", "a8086", "javascript", "plsql" };
        public volatile bool InUse = false;

        public const int DEF_SHOW = 250;
        public const int DEF_MAX_FOUND = 10;
        public readonly bool IsExperimental = false;

        // private const string RESULTS = "https://moss.stanford.edu/results/";
        private class MossRequestor : IDisposable
        {
            private const int ID = 756989629;
            //private const string TEST_SERVER = "127.0.0.1";
            //private const int TEST_PORT = 8000;
            private readonly ApiSocket socket;
            private const string SERVER = "moss.stanford.edu";
            private const int PORT = 7690;
            private readonly MossData data;
            private uint id = 1;

            /// <summary>
            ///     Constructor. Takes MossData object.
            /// </summary>
            /// <param name="d">The parameters of the moss request.</param>
            public MossRequestor(MossData d, bool test = false)
            {
                data = d;
                socket = new ApiSocket();
                socket.TestMode = test;
            }
            /// <summary>
            ///     Attempts to connect to moss.stanford.edu.
            /// </summary>
            /// <exception cref="SocketException">
            ///     There was an issue connecting to the server.
            /// </exception>
            /// 
            public void Connect()
            {
                socket.Connect(SERVER, PORT);
            }
            /// <summary>
            ///     Sends the request parameters to the moss server
            /// </summary>
            /// <param name="isExp">Whether the experimental Moss server should be used</param>
            /// <returns>Either "yes" or "no" from the server</returns>
            public void SendHeader()
            {
                socket.SendCommandLine($"moss {ID}");
                socket.SendCommandLine("directory 1");
                socket.SendCommandLine($"X {Convert.ToInt32(data.IsExperimental)}");
                socket.SendCommandLine($"maxmatches {data.MaxFound}");
                socket.SendCommandLine($"show {data.MatchesShow}");
                socket.SendCommandLine($"language {data.Language}");
                string langReponse = socket.ReadLineFromServer().Trim();
                if(langReponse == "no")
                {
                    throw new NotSupportedException($"It appears Moss doesn't support {data.Language}.");
                }  
            }
            /// <summary>
            ///     Uploads the files in the given folder to the server.
            /// </summary>
            /// <param name="folder">the containing folder</param>
            /// <param name="isBaseFiles">Whether the folder contains base files or submission files</param>
            private void UploadFiles(string folder, bool isBaseFiles = false)
            {
                // Simplifying directory name
                
                var files = FileUtils.GetFiles(folder, FileUtils.LangToExt[data.Language]);
                string dirname = FileUtils.GetFileName(folder);
                foreach (string file in files)
                {
                    // Simplifying file name
                    string filename = FileUtils.FlattenFilePath(dirname, file);
                    filename = filename.Replace(' ', '_');
                    uint fileId = isBaseFiles ? 0 : id++;
                    string fileContent = File.ReadAllText(file);
                    string command = $"file {fileId} {data.Language} {fileContent.Length} {filename}";
                    socket.SendCommandLine(command);
                    socket.SendCommand(fileContent);
                }
            }
            public void UploadBaseFiles() {
                if(data.BaseFilesFolder != null) {
                     UploadFiles(data.BaseFilesFolder, true);
                }
            }
            /// <summary>
            ///     Uploads all excercise submissions to the server
            /// </summary>
            /// <param name="dir">The submissions directory.</param>
            public void UploadSubmissions()
            {
                string[] subFolders = Directory.GetDirectories(data.SubmissionsFolder);
                foreach (string folder in subFolders)
                {
                    UploadFiles(folder);
                }
            }
            /// <summary>
            ///     Requesting a result from the server.
            /// </summary>
            /// <returns>The link to the check results.</returns>
            public string GetResult()
            {
                socket.SendCommandLine($"query 0 {data.Comment}");
                string result = socket.ReadLineFromServer().Trim();
                socket.SendCommandLine("end");
                id = 1;
                return result;
            }

            public void Dispose() {
                socket.Dispose();
            }

        }

        public MossClient(bool isExp = false) {
            IsExperimental = isExp;
        }
        /// <summary>method <c>IsSupported</c> Checks if a language is supported by Moss.</summary>
        /// <param name="language">the language to be checked.</param>
        /// <returns>True if the langauge is supported. False otherwise.</returns>
        public static bool IsSupported(string language)
        {
            return (FileUtils.LangToExt[language] != null && SupportedLangauges.Contains(language));
        }
        /// <summary>
        ///     Checks if the url is valid.
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>True if the URL is valid. False otherwise.</returns>
        private static bool CheckUrl(string url)
        {
            bool valid = Uri.TryCreate(url, UriKind.Absolute, out Uri result);
            if (valid == true && result != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Sends a request to the moss server.
        /// </summary>
        /// <param name="data">Parameter object for the Moss request parameters</param>
        /// <param name="folder">Folder containing the submissions</param>
        /// <param name="baseFolder">Folder containing the base files</param>
        /// <exception href="NotSupportedException">
        ///     If the entered language is not supported by Moss.
        /// </exception>
        /// <exception href="FileNotFoundException">
        ///     IIf the submission or base file folder aren't found.
        /// </exception>
        /// <exception href="Exception">
        ///     If the Moss server returns a result that isn't a working link.
        /// </exception>
        /// <returns>The results URL</returns>
        public string SendRequest(MossData data, bool testMode = false)
        {
            data.Language = data.Language.ToLower();
            if (!IsSupported(data.Language))
            {
                throw new NotSupportedException($"The langauge {data.Language} isn't supported.");
            }
            if(!Directory.Exists(data.SubmissionsFolder))
            {
                throw new FileNotFoundException("Submissions folder not found.");
            }
            if(!Directory.Exists(data.BaseFilesFolder) && data.BaseFilesFolder != null) 
            {
                throw new FileNotFoundException("Base files folder not found.");
            }
            // Converts inapropriate values into default values
            data.MaxFound = (data.MaxFound < 2 ? DEF_MAX_FOUND : data.MaxFound);
            data.MatchesShow = (data.MatchesShow < 1 ? DEF_SHOW : data.MatchesShow);
            string result;
            using(MossRequestor request = new MossRequestor(data, testMode))
            {        
                    request.Connect();
                    request.SendHeader();
                    request.UploadBaseFiles();
                    request.UploadSubmissions();
                    result = request.GetResult();
            }
            if (!CheckUrl(result))
            {
                // Console.WriteLine(result);
                throw new Exception("The Moss server didn't return a working link. Returned result was: " + result );
            }
            return result;
        }
    }
}