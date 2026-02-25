using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Jobs
{
    public class RemoveExpiredRefreshTokensJob : IRemoveExpiredRefreshTokensJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveExpiredRefreshTokensJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            await _unitOfWork.RefreshTokens.RemoveExpiredRefreshTokensAsync();
            await _unitOfWork.CommitAsync();
        }
    }
}
