using Pluto.Repository.Core.Domain;
using Pluto.Repository.Core.Repositories;
using System.Data.Entity;
using System.Linq;

namespace Pluto.Repository.Persistence.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(PlutoContext context) : base(context)
        {
        }

        public Author GetAuthorWithCourses(int id)
        {
            return PlutoContext.Authors.Include(a => a.Courses).SingleOrDefault(a => a.Id == id);
        }

        public PlutoContext PlutoContext
        {
            get { return Context as PlutoContext; }
        }
    }
}