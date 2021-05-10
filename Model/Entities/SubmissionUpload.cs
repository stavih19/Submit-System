using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
namespace Submit_System {
    public class SubmissionUpload {
        
        public List<IFormFile> files {get; set;}
        public List<Student> Submitters {get; set; }
    }
}
