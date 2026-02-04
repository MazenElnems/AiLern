using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Domain.Interfaces
{
    public interface IMaterialFileRepository : IBaseRepository<MaterialFile>
    {
        public int GetMaxOrderIndex(Guid sectionId);

        public void DeleteFile(MaterialFile file);
    }
}
