using LMS.Application.ConfigurationOptions;
using LMS.Domain.DTOs.MaterialFiles;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
