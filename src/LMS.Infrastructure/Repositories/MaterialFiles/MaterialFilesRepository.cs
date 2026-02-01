using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories.MaterialFiles;

internal class MaterialFileRepository : BaseRepository<MaterialFile>, IMaterialFileRepository
{
    private readonly AppDbContext _context;
    public MaterialFileRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
