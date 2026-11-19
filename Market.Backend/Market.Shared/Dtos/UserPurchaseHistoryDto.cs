namespace Market.Shared.Dtos
{
    /// <summary>
    /// Historija kupovina jednog korisnika (admin pregled i PDF izvještaj):
    /// podaci o korisniku + statistika + narudžbe u zadanom periodu.
    /// </summary>
    public class UserPurchaseHistoryDto
    {
        // ---- korisnik ----
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public DateTime RegisteredAt { get; set; }

        // ---- period (null = bez ograničenja) ----
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        // ---- statistika za period (otkazane narudžbe se ne računaju u potrošnju) ----
        public int OrdersCount { get; set; }
        public int GamesCount { get; set; }
        public decimal TotalSpent { get; set; }

        public List<OrderHistoryDto> Orders { get; set; } = new();
    }
}
