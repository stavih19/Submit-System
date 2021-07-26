using System;

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
        
    }
}