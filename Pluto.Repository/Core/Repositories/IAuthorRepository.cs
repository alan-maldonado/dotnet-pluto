using Pluto.Repository.Core.Domain;

namespace Pluto.Repository.Core.Repositories
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Author GetAuthorWithCourses(int id);
    }
}