namespace lps_web_test.Domain.Entities;

public partial class BooksAudit
{
    public long AuditId { get; set; }

    public int BookId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public string? ActionBy { get; set; }

    public DateTime? ActionAt { get; set; }
}
