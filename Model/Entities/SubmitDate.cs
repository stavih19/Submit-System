using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Submit_System {
    public class SubmitDate {
        public DateTime Date { get; set;}
        [JsonIgnore]
        public int ID { get; set; }
        public string ExerciseID { get; set; }
        public int Group {get; set; }
        public int Reduction { get; set; }
        public int CompareTo(SubmitDate sb)
        {
            return Date.CompareTo(sb.Date);
        }

    }
}
