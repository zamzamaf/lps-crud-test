using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lps_crud_test.Models.LpsDb;

public partial class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Book Title is required")]
    public string? BookTitle { get; set; }

    [Required(ErrorMessage = "Author is required")]
    public string? Author { get; set; }

    [Required(ErrorMessage = "Book type is required")]
    public int? BookTypeId { get; set; }

    [Required(ErrorMessage = "Release date is required")]
    public DateTime? ReleaseDate { get; set; }

    [Required(ErrorMessage = "Number of pages is required")]
    public int? NumberOfPages { get; set; }

    public BitArray? IsActive { get; set; }

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

    public virtual BookType? BookType { get; set; }
}
