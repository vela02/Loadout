using MediatR;
using System.Collections.Generic;

namespace Market.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommand : IRequest<int>
{
    
    public string Title { get; set; } = string.Empty; 
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? Genre { get; set; }

    
    public string? ImageUrl { get; set; }
    public string? TrailerUrl { get; set; }

    
    public DateOnly? ReleaseDate { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public string? SystemRequirements { get; set; } 

    
    public List<int>? TagIds { get; set; }
}