namespace Market.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommandHandler(IAppDbContext ctx)
            : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        
        var entity = await ctx.Products
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new MarketNotFoundException($"Proizvod (ID={request.Id}) nije pronađen.");

        // check for title uniqueness
        var exists = await ctx.Products
            .AnyAsync(x => x.Id != request.Id && x.Title == request.Title, ct);

        if (exists)
        {
            throw new MarketConflictException($"Igra s nazivom '{request.Title}' već postoji.");
        }


        // check if category exists
        var productCategory = await ctx.ProductCategories
          .Where(x => x.Id == request.CategoryId)
          .FirstOrDefaultAsync(ct);

        if (productCategory is null)
        {
            throw new ValidationException("Nepostojeća kategorija.");
        }


        // mapping  
        entity.Title = request.Title.Trim();
        entity.Description = request.Description?.Trim();
        entity.Price = request.Price;
        entity.CategoryId = request.CategoryId;
        entity.Genre = request.Genre;

        entity.ImageUrl = request.ImageUrl;
        entity.TrailerUrl = request.TrailerUrl;

        entity.ReleaseDate = request.ReleaseDate;
        entity.Developer = request.Developer;
        entity.Publisher = request.Publisher;
        entity.SystemRequirements = request.SystemRequirements;


        // adding tags
        if (request.TagIds != null)
        {
            entity.Tags.Clear(); 

            if (request.TagIds.Count > 0)
            {
                var tags = await ctx.GameTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync(ct);

                foreach (var tag in tags)
                {
                    entity.Tags.Add(tag);
                }
            }
        }
     
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}