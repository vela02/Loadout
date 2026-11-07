namespace Market.Shared.Dtos
{
    /// <summary>Vijest kako je vidi administrator (uključuje status aktivnosti).</summary>
    public class AnnouncementAdminDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
