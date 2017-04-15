using Pluto.Repository.Core.Repositories;
using System;

namespace Pluto.Repository.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Courses { get; }
        IAuthorRepository Authors { get; }
        int Complete();
    }
}