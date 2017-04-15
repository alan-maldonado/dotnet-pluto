using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.DbFirst
{
    public enum Level : byte
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }

    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new PlutoDbContext();
            var courses = dbContext.GetCourses();
            foreach (var c in courses)
            {
                Console.WriteLine(c.Title);
            }

            var course = new Course();
            course.Level = Level.Intermediate;

        }
    }
}
