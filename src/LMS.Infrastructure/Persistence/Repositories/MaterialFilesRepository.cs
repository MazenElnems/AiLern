using LMS.Domain.Entities.Courses;
using LMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class MaterialFileRepository : BaseRepository<MaterialFile>, IMaterialFileRepository
{
    private readonly AppDbContext _context;
    public MaterialFileRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public void DeleteFile(MaterialFile file)
    {
        _context.MaterialFiles.Remove(file);

        var filesToShift = _context.MaterialFiles
                .Where(f => f.SectionId == file.SectionId &&
                            f.OrderIndex > file.OrderIndex).ToList();
        foreach (var f in filesToShift)
        {
            f.OrderIndex -= 1;
        }
    }

    public async Task<int> GetMaxOrderIndexAsync(Guid sectionId)
    {
        return await _context.MaterialFiles
                .Where(f => f.SectionId == sectionId)
                .MaxAsync(f => (int?)f.OrderIndex) ?? 0;
    }


}
