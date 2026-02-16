using LMS.Domain.Entities.Courses;
using LMS.Domain.Repositories;

namespace LMS.Domain.Interfaces;

// if the repository empty then use 
// IBaseRepository<Section> as property type in unitOfWork
public interface ISectionRepository : IBaseRepository<Section>
{

}
