using Market.Domain.Models;
using Market.Shared.Enums;

namespace Market.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // check if game with the same title already exists
        if (await ctx.Products.AnyAsync(x => x.Title == request.Title, ct))
        {
            throw new Exception($"Igra '{request.Title}' već postoji.");
        }

        // create new game entity
        var game = new Game
        { 
        
            Title = request.Title, 
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            Genre = request.Genre,

            ImageUrl = request.ImageUrl,
            TrailerUrl = request.TrailerUrl,

            ReleaseDate = request.ReleaseDate,
            Developer = request.Developer,
            Publisher = request.Publisher,
            SystemRequirements = request.SystemRequirements,

            IsEnabled = true,
            IsDeleted = false,
            ContentType = GameContentType.Game,

            
            Tags = new List<GameTag>()
        };

        // associate tags if provided
        if (request.TagIds != null && request.TagIds.Count > 0)
        {
            var tags = await ctx.GameTags
                .Where(t => request.TagIds.Contains(t.Id))
                .ToListAsync(ct);

            foreach (var tag in tags)
            {
                game.Tags.Add(tag);
            }
        }

        
        ctx.Products.Add(game);
        await ctx.SaveChangesAsync(ct);

        return game.Id;
    }
 }
