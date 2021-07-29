using System;
using System.IO;

namespace Submit_System
{
    public class SubmitFile
    {
        public string Name { get; set; }
        public string Content { get; set; }

        public static SubmitFile Create(string name, byte[] content)
        {
            var file = new SubmitFile {
                Name = name,
                Content = Convert.ToBase64String(content)
            };
            return file;
        }
        public static SubmitFile Create(string path)
        {
            if(!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            string content = Convert.ToBase64String(File.ReadAllBytes(path));
            string name = FileUtils.GetFileName(path);
            return new SubmitFile {
                Name = name,
                Content = content
            };
        }
        public void CreateFile(string folder)
        {
            string path = Path.Combine(folder, Name);
            byte[] bytes = System.Convert.FromBase64String(Content);
            using (var stream = File.Create(path))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}