namespace lps_web_test.Domain.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string? BookTitle { get; set; }

    public string? Author { get; set; }

    public int? BookTypeId { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? NumberOfPages { get; set; }

    public virtual BookType? BookType { get; set; }
}
