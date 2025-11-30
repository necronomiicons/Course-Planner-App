using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Planner_App
{
    [Table("Class")]
    public class Class
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement, Column("_id")]
        public int classId { get; set; }
        public string className {  get; set; }
        public int activeAssignments { get; set; }

    }
}
