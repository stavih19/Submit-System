using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Submit_System {

    public class TestInput{
        Test test {get; set;}
        List<SubmitFile> files {get; set;}
    }
}