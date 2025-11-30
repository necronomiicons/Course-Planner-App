using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Planner_App
{
    public enum ExamType { Quiz, Project, Test }
    public class Exam : Assessment
    {
        public ExamType examType {  get; set; }

        public override string GetAssessmentType
        {
            get
            {
                return examType.ToString();
            }
        }
    }
}
