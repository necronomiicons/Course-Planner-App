using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Planner_App
{
    public class Helper
    {
        public static string GetClassNameFromId(int id)
        {
            foreach(Class c in MainPage.database.Table<Class>().ToList())
            {
                if(c.classId == id)
                {
                    return c.className;
                }    
            }
            return null;
        }


        //Exception Handling
        public static bool ClassIsUnique(string className)
        {
            foreach(Class c in MainPage.classList)
            {
                if(c.className == className)
                {
                    return false;
                }
            }
            return true;
        }
        public static  bool ValidStringInput(string input)
        {
            if(input != null && input.Trim().Length > 0)//string cannot be empty
            {
                return true;
            }
            return false;
        }
        public static bool ValidDateTimeInput(DateTime dateTime)
        {
            if(dateTime >= DateTime.Now) //date cannot be in the past
            {
                return true;
            }
            return false;
        }

    }
}
