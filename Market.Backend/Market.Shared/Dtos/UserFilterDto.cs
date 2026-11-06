namespace Market.Shared.Dtos
{
    /// <summary>
    /// Parametri za admin listu korisnika: 5 filtera + sortiranje + paging.
    /// Šalju se kao query string: /api/admin/users/paged?search=ana&amp;roleId=2&amp;page=1...
    /// </summary>
    public class UserFilterDto
    {
        // ---- 5 filter parametara ----
        public string? Search { get; set; }          // 1. username ili email
        public int? RoleId { get; set; }             // 2. uloga
        public bool? IsEnabled { get; set; }         // 3. status (aktivan / blokiran)
        public DateTime? CreatedFrom { get; set; }   // 4. registrovan od
        public DateTime? CreatedTo { get; set; }     // 5. registrovan do

        // ---- sortiranje po koloni ----
        public string? SortBy { get; set; } = "createdAt"; // username | email | role | createdAt | lastLogin | status
        public string? SortDir { get; set; } = "desc";      // asc | desc

        // ---- paging ----
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
