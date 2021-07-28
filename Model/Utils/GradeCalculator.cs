using System;
using System.Collections.Generic;
using System.Linq;
namespace Submit_System {
    public class GradeCalculator {
        
        public int StyleGrade {get; set; }
        public int AutoGrade {get; set; }
        public int ManualGrade {get; set; }
        public int StyleWeight {get; set; } 
        public int AutoWeight {get; set; }
        public bool copied { get; set; } = false;
        public int[] Reductions {get; set;}
        public int InitReduction {get; set; } = 0;
        public int MaxLateDays { get => Reductions?.Length ?? 0; }
        public DateTime Date {get; set;}
        public DateTime TimeSubmitted {get; set; }
       
        public int CalculateGrade()
        {
            if(copied)
            {
                return 0;
            }
            int diff = (TimeSubmitted - Date).Days;
            if(Reductions.Length < diff)
            {
                return 0;
            }
            int manualWeight = 100 - StyleWeight - AutoWeight;
            int reduction = diff > 0 ? Reductions[diff-1] : 0;
            int grade = ((StyleGrade * StyleWeight) + (AutoGrade * AutoWeight) + (ManualGrade * manualWeight)) / 100;
            int total = grade - InitReduction - reduction;
            return Math.Max(total, 0);
        }
    }
}
