namespace Market.Application.Modules.Sales.Orders.Queries.List;

public sealed class ListOrdersQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
        : IRequestHandler<ListOrdersQuery, PageResult<ListOrdersQueryDto>>
{

    public async Task<PageResult<ListOrdersQueryDto>> Handle(ListOrdersQuery request, CancellationToken ct)
    {  
        var query = ctx.Orders.AsNoTracking();

        // if not admin, filter by current user
        if (!currentUser.IsAdmin)
        {         
           query = query.Where(x => x.UserId == currentUser.UserId);
        }

        // search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(x => x.Id.ToString().Contains(term));
        }

        // filters
        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == request.StatusId);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(x => x.Date >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(x => x.Date <= request.DateTo.Value);
        }

        // data projection
        var projectedQuery = query
            .OrderByDescending(x => x.Date) 
            .Select(x => new ListOrdersQueryDto
            {
                Id = x.Id,
                ReferenceNumber = $"ORD-{x.Id}",
                OrderedAtUtc = x.Date,
                PaidAtUtc = x.Date,              
                Status = x.Status != null ? x.Status.Name : "-",

                TotalAmount = x.TotalAmount ?? 0,
                Note = "",

                User = new ListOrdersQueryDtoUser
                {
                    UserFirstname = x.User != null ? x.User.Username : "Guest",
                    UserLastname = x.User != null ? x.User.Email : "",
                    UserAddress = "-",
                    UserCity = "-"
                }
            });
     
        return await PageResult<ListOrdersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);         
    }
}
