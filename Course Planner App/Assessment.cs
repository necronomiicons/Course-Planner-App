using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Planner_App
{
    public enum AssessmentType { Exam, Assignment }

    [Table("Assessment")]
    public class Assessment
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement, Column("_id")]
        public int assessmentId { get; set; }
        public string assessmentName { get; set; }
        public AssessmentType assessmentType { get; set; }
        public int classId { get; set; }
        public DateTime dueDate { get; set; }
        public DateTime completionDate { get; set; }
        public bool enableNotifs { get; set; }
        public bool complete {  get; set; }

        public bool Incomplete
        {
            get { return !complete; }
        }

        public void Complete()
        {
            Trace.WriteLine("Success");
            completionDate = DateTime.Now;
            complete = true;
        }

        public virtual string GetAssessmentType
        {
            get
            {
                return assessmentType.ToString();
            }
        }
        public string GetClassName
        {
            get
            {
                foreach (Class c in MainPage.classList)
                {
                    if (c.classId == classId)
                    {
                        return c.className;
                    }
                }
                return "";
            }
        }

        public string GetDaysLate
        {
            get
            {
                TimeSpan days = (completionDate - dueDate);
                if(days.Days > 0)
                {
                    return days.Days + " days";
                }
                else if(days.Hours > 0)
                {
                    return days.Hours + " hours";
                }
                else
                {
                    return days.Minutes + " minutes";
                }
            }
        }
        
    }
}
