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

    public void DeleteFile(MaterialFile file)
    {
        _context.Set<MaterialFile>().Remove(file);
    }

    public int GetMaxOrderIndex(Guid sectionId)
    {
        var files = _context.MaterialFiles.Where(f => f.SectionId == sectionId);
        if (files.Count() == 0)
            return 0;
        return files.Max(f => f.OrderIndex);

    }
}
