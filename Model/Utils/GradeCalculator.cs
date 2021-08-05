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
            if(AutoGrade < 0 || ManualGrade < 0)
            {
                return -1;
            }
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
        public void SetDate(List<SubmitDate> dates)
        {
            SubmitDate realDate = null;
            int reduction = 100;
            int len = dates.Count;
            for(int i = 0; i < len; i++)
            {
                var date = dates[i];
                if(date.Date >= this.TimeSubmitted.Date)
                {
                    if(date.Reduction < reduction)
                    {
                        realDate = date;
                        reduction = date.Reduction;
                    }
                    break;
                }
                else if(date.Date.AddDays(this.MaxLateDays) >= this.TimeSubmitted.Date)
                {
                    var diff = (this.TimeSubmitted.Date - date.Date).Days;
                    int newReduction = this.Reductions[diff-1] + date.Reduction;
                    if(newReduction < reduction)
                    {
                        reduction = newReduction;
                        realDate = date;
                    }
                }
            }
            if(realDate == null)
            {
                Date = dates.Last().Date;
                InitReduction = dates.Last().Reduction;
            }
            else
            {
                Date = realDate.Date;
                InitReduction = realDate.Reduction;
            }
        }
    }
}
