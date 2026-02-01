using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories.Sections
{
    internal class SectionRepository : BaseRepository<Section>, ISectionRepository
    {
        private readonly AppDbContext _context;
        public SectionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
