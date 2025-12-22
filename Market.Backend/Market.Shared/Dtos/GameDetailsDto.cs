using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class GameDetailsDto:GameListDto
    {
        public string? Description { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
        public string? TrailerUrl { get; set; }
        public string? SystemRequirements { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public List<CommentDto> Reviews { get; set; }=new List<CommentDto>();
    }
}
