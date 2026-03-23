using AutoMapper;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Attempts.Shared.Mappings;

public class AttemptProfile : Profile
{
    public AttemptProfile()
    {
        CreateMap<Attempt, GetAttemptByIdDto>();
        CreateMap<AttemptAnswer, AttemptResultDto>();
    }
}
