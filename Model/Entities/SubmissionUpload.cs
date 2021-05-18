using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
namespace Submit_System {
    public class SubmissionUpload {
        
        public List<IFormFile> Files {get; set;}
        public List<string> Submitters {get; set; }
    }
}
