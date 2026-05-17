using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.ProcessAIUploadHook;

public record ProcessAIUploadWebhookCommand(AIWebhookDto Dto) : IRequest;
