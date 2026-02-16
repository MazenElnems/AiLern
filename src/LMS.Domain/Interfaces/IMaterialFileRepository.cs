using LMS.Domain.Entities.Courses;
using LMS.Domain.Repositories;

namespace LMS.Domain.Interfaces
{
    public interface IMaterialFileRepository : IBaseRepository<MaterialFile>
    {
        public Task<int> GetMaxOrderIndexAsync(Guid sectionId);

        public void DeleteFile(MaterialFile file);

    }
}
