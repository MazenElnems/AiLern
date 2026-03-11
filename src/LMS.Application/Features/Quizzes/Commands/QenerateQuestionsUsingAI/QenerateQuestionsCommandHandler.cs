using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;

public class QenerateQuestionsCommandHandler : IRequestHandler<QenerateQuestionsCommand, object>
{
    public async Task<object> Handle(QenerateQuestionsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
