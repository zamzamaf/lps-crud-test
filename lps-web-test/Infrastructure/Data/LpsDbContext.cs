using lps_web_test.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace lps_web_test.Infrastructure.Data;

public partial class LpsDbContext : DbContext
{
    public LpsDbContext(DbContextOptions<LpsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookType> BookTypes { get; set; }

    public virtual DbSet<BooksAudit> BooksAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_books");

            entity.ToTable("books");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .HasColumnName("author");
            entity.Property(e => e.BookTitle)
                .HasMaxLength(100)
                .HasColumnName("book_title");
            entity.Property(e => e.BookTypeId).HasColumnName("book_type_id");
            entity.Property(e => e.NumberOfPages).HasColumnName("number_of_pages");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("release_date");

            entity.HasOne(d => d.BookType).WithMany(p => p.Books)
                .HasForeignKey(d => d.BookTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_books_book_type");
        });

        modelBuilder.Entity<BookType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_book_types");

            entity.ToTable("book_types");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BookTypeName)
                .HasMaxLength(50)
                .HasColumnName("book_type_name");
        });

        modelBuilder.Entity<BooksAudit>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("books_audit_pkey");

            entity.ToTable("books_audit");

            entity.Property(e => e.AuditId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("audit_id");
            entity.Property(e => e.ActionAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("action_at");
            entity.Property(e => e.ActionBy)
                .HasMaxLength(50)
                .HasColumnName("action_by");
            entity.Property(e => e.ActionType)
                .HasMaxLength(10)
                .HasColumnName("action_type");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.NewData)
                .HasColumnType("jsonb")
                .HasColumnName("new_data");
            entity.Property(e => e.OldData)
                .HasColumnType("jsonb")
                .HasColumnName("old_data");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
