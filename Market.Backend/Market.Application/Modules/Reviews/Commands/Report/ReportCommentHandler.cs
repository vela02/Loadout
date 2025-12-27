using MediatR;

namespace Market.Application.Modules.Reviews.Commands.Report;

public sealed class ReportCommentCommandHandler(IAppDbContext ctx)
    : IRequestHandler<ReportCommentCommand, bool>
{
    public async Task<bool> Handle(ReportCommentCommand request, CancellationToken ct)
    {
        // find the comm
        var comment = await ctx.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, ct);

        
        if (comment == null)
        {
            return false;
        }
        
        comment.IsReported = true;

        await ctx.SaveChangesAsync(ct);

        return true;
    }
}