using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lps_crud_test.Models.LpsDb;

public partial class BookType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Book Type Name is required")]
    public string? BookTypeName { get; set; }

    public BitArray? IsActive { get; set; }

    /// <summary>
    /// Convenience boolean wrapper for the BitArray field used in the database.
    /// This property is not mapped to EF and is used by views/controllers.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsActiveBool
    {
        get => IsActive != null && IsActive.Length > 0 && IsActive[0];
        set => IsActive = new BitArray(new[] { value });
    }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
