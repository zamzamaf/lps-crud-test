namespace lps_web_test.Domain.Entities;

public partial class BookType
{
    public int Id { get; set; }

    public string? BookTypeName { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
