namespace Market.Shared.Dtos
{
    /// <summary>Admin lista vijesti: 5 filtera + paging (query string).</summary>
    public class AnnouncementFilterDto
    {
        public string? Title { get; set; }           // 1. pretraga po naslovu
        public string? Type { get; set; }            // 2. Sale | Info | Maintenance | Update
        public bool? IsActive { get; set; }          // 3. aktivna / neaktivna
        public DateTime? CreatedFrom { get; set; }   // 4. objavljena od
        public DateTime? CreatedTo { get; set; }     // 5. objavljena do

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
