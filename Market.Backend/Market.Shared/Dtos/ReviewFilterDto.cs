namespace Market.Shared.Dtos
{
    /// <summary>Admin moderacija recenzija: filteri + paging (query string).</summary>
    public class ReviewFilterDto
    {
        public string? Game { get; set; }            // 1. naziv igre (sadrži)
        public string? User { get; set; }            // 2. korisničko ime (sadrži)
        public int? MinRating { get; set; }          // 3. minimalna ocjena autora (1–5)
        public DateTime? CreatedFrom { get; set; }   // 4. napisano od
        public DateTime? CreatedTo { get; set; }     // 5. napisano do
        public bool? ReportedOnly { get; set; }      // 6. samo prijavljene recenzije

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
