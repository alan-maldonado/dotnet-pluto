using System;
using System.Data.Entity;
using System.Linq;

namespace Pluto.Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            ChangeTracker();
        }

        static void Linq()
        {
            var context = new PlutoContext();

            // LINQ syntax
            Console.WriteLine("LINQ syntax...");
            var query =
                from c in context.Courses
                where c.Name.Contains("c#")
                orderby c.Name
                select c;

            foreach (var course in query)
            {
                Console.WriteLine(course.Name);
            }

            // Extension methods
            Console.WriteLine("Extension methods...");
            var courses = context.Courses
                .Where(c => c.Name.Contains("c#"))
                .OrderBy(c => c.Name);

            foreach (var course in courses)
            {
                Console.WriteLine(course.Name);
            }
        }

        static void Linq2()
        {
            var context = new PlutoContext();

            // Simple
            var query =
                from c in context.Courses
                where c.Author.Id == 1
                orderby c.Level descending, c.Name
                select new { Name = c.Name, Author = c.Author.Name };

            // Grouping

            var query2 =
                from c in context.Courses
                group c by c.Level
                into g
                select g;

            foreach (var group in query2)
            {
                Console.WriteLine("{0} ({1})", group.Key, group.Count());

                foreach (var course in group)
                {
                    Console.WriteLine("\t{0}", course.Name);
                }
            }

            // Joining
            var query3 =
                from c in context.Courses
                join a in context.Authors on c.AuthorId equals a.Id
                select new { CourseName = c.Name, AuthorName = a.Name };

            // Group join - left join
            var query4 =
                from a in context.Authors
                join c in context.Courses on a.Id equals c.AuthorId into g
                select new { AuthorName = a.Name, Courses = g.Count() };

            foreach (var x in query4)
            {
                Console.WriteLine("{0} ({1})", x.AuthorName, x.Courses);
            }

            // cross join
            var query5 =
                from a in context.Authors
                from c in context.Courses
                select new { AuthorName = a.Name, CourseName = c.Name };

            foreach (var x in query5)
            {
                Console.WriteLine("{0} ({1})", x.AuthorName, x.CourseName);
            }
        }

        static void ExtensionMethods()
        {
            var context = new PlutoContext();

            // Simples 
            var courses = context.Courses
                .Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name).ThenBy(c => c.Level)
                .Select(c => new { CourseName = c.Name, AuthorName = c.Author.Name});

            var tags = context.Courses
                .Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name).ThenBy(c => c.Level)
                .SelectMany(c => c.Tags)
                .Distinct();

            // Groups
            var groups = context.Courses.GroupBy(c => c.Level);

            foreach (var group in groups)
            {
                Console.WriteLine("Key: " + group.Key);
                foreach (var course in group)
                {
                    Console.WriteLine("\t" + course.Name);
                }
            }

            // Join
            context.Courses.Join(context.Authors,
                c => c.AuthorId,
                a => a.Id,
                (course, author) => new
                {
                    CourseName = course.Name,
                    AuthorName = author.Name
                });

            // Group Join
            context.Authors.GroupJoin(context.Courses,
                a => a.Id,
                c => c.AuthorId,
                (author, coursesList) => new
                {
                    Author = author.Name,
                    Courses = coursesList.Count()
                });

            // Cross join
            context.Authors.SelectMany(a => context.Courses,
                (author, course) => new
                {
                    AuthorName = author.Name,
                    CourseName = course.Name
                });


            // Partitioning
            context.Courses.Skip(10).Take(10);

            // Element Operators
            context.Courses.OrderBy(c => c.Level).FirstOrDefault(c => c.FullPrice > 100);
            context.Courses.OrderBy(c => c.Level).SingleOrDefault(c => c.Id == 1);

            // Quantifing
            bool r = context.Courses.All(c => c.Level == 1);
            r = context.Courses.Any(c => c.Level == 1);

            // Aggregating
            context.Courses.Count();
            context.Courses.Max(c => c.FullPrice);
            context.Courses.Min(c => c.FullPrice);
            context.Courses.Average(c => c.FullPrice);

        }

        static void Deffered()
        {
            var context = new PlutoContext();

            var courses = context.Courses;
            var filtered = courses.Where(c => c.Level == 1);
            var sorted = filtered.OrderBy(c => c.Name);

            foreach (var c in courses)
            {
                Console.WriteLine(c.Name);
            }
        }

        static void IqueryableExample()
        {
            var context = new PlutoContext();
            IQueryable<Course> courses = context.Courses;

            var filtered = courses.Where(c => c.Level == 1);
        }

        static void LazyLoading()
        {
            var context = new PlutoContext();
            var courses = context.Courses.ToList();

            foreach (var course in courses)
            {
                Console.WriteLine("{0} by {1}", course.Name, course.Author.Name);
            }
        }

        static void EagerLoading()
        {
            var context = new PlutoContext();
            // var courses = context.Courses.Include("Author").ToList(); // bad practice
            var courses = context.Courses.Include(c => c.Author).ToList();

            foreach (var course in courses)
            {
                Console.WriteLine("{0} by {1}", course.Name, course.Author.Name);
            }


            //context.Courses.Include(c => c.Author.Address);
            //context.Courses.Include(a => a.Tags.Select(t => t.Name));
        }

        static void ExplicitLoading()
        {
            var context = new PlutoContext();

            var author = context.Authors.Single(a => a.Id == 1);

            // MSDN way
            context.Entry(author).Collection(a => a.Courses).Query().Where(c => c.FullPrice == 0).Load();

            // Desired way
            context.Courses.Where(c => c.AuthorId == author.Id && c.FullPrice == 0).Load();

            foreach (var course in author.Courses)
            {
                Console.WriteLine("{0}", course.Name);
            }
        }

        static void ChangingData()
        {
            AddingData();
        }

        static void AddingData()
        {
            var context = new PlutoContext();
            var author = context.Authors.Single(a => a.Id == 1);

            var course = new Course
            {
                Name = "New Course",
                Description = "New Description",
                FullPrice = 19.95f,
                Level = 1,
                Author = author
            };

            context.Courses.Add(course);

            context.SaveChanges();
        }

        static void UpdateData()
        {
            var context = new PlutoContext();
            var course = context.Courses.Find(1);
            course.Name = "New course name";
            course.AuthorId = 2;

            context.SaveChanges();
        }

        static void RemoveData()
        {
            var context = new PlutoContext();
            var course = context.Courses.Find(6);
            context.Courses.Remove(course);

            context.SaveChanges();
        }

        static void ChangeTracker()
        {
            var context = new PlutoContext();

            context.Authors.Add(new Author { Name = "New Author" });

            var author = context.Authors.Find(3);
            author.Name = "Updated";

            var another = context.Authors.Find(4);
            context.Authors.Remove(another);

            var entries = context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                entry.Reload();
                Console.WriteLine(entry.State);
            }
               
        }
    }
}
