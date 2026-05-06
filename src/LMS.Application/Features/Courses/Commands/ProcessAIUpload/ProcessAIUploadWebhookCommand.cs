using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.ProcessAIUpload;

public record ProcessAIUploadWebhookCommand(AIWebhookDto Dto) : IRequest;
