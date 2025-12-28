
namespace Market.Application.Modules.Reviews.Commands.Report;

public record ReportCommentCommand(int CommentId) : IRequest<bool>;