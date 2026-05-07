using Microsoft.EntityFrameworkCore;
using lps_web_test.Domain.Entities;
using lps_web_test.Infrastructure.Data;
using lps_web_test.Infrastructure.Repositories;
using Xunit;

namespace lps_web_test.Tests;

public class BookTypeRepositoryTests
{
    private static DbContextOptions<LpsDbContext> CreateOptions(string dbName)
    {
        return new DbContextOptionsBuilder<LpsDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBookTypes()
    {
        var options = CreateOptions(nameof(GetAllAsync_ReturnsAllBookTypes));

        await using (var context = new LpsDbContext(options))
        {
            context.BookTypes.AddRange(
                new BookType { BookTypeName = "Fiction" },
                new BookType { BookTypeName = "Nonfiction" });
            await context.SaveChangesAsync();
        }

        await using (var context = new LpsDbContext(options))
        {
            var repository = new BookTypeRepository(context);
            var result = await repository.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, bt => bt.BookTypeName == "Fiction");
            Assert.Contains(result, bt => bt.BookTypeName == "Nonfiction");
        }
    }

    [Fact]
    public async Task GetAllAsync_SearchFiltersByName()
    {
        var options = CreateOptions(nameof(GetAllAsync_SearchFiltersByName));

        await using (var context = new LpsDbContext(options))
        {
            context.BookTypes.AddRange(
                new BookType { BookTypeName = "Science Fiction" },
                new BookType { BookTypeName = "History" },
                new BookType { BookTypeName = "Science" });
            await context.SaveChangesAsync();
        }

        await using (var context = new LpsDbContext(options))
        {
            var repository = new BookTypeRepository(context);
            var query = repository.GetAllAsync("Science");
            var result = await query.ToListAsync();

            Assert.Equal(2, result.Count);
            Assert.All(result, bt => Assert.Contains("Science", bt.BookTypeName ?? string.Empty, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMatchingBookType()
    {
        var options = CreateOptions(nameof(GetByIdAsync_ReturnsMatchingBookType));

        await using (var context = new LpsDbContext(options))
        {
            context.BookTypes.Add(new BookType { BookTypeName = "Drama" });
            await context.SaveChangesAsync();
        }

        await using (var context = new LpsDbContext(options))
        {
            var repository = new BookTypeRepository(context);
            var entity = await repository.GetByIdAsync(1);

            Assert.NotNull(entity);
            Assert.Equal("Drama", entity!.BookTypeName);
        }
    }

    [Fact]
    public async Task AddAsync_PersistsBookType()
    {
        var options = CreateOptions(nameof(AddAsync_PersistsBookType));

        await using (var context = new LpsDbContext(options))
        {
            var repository = new BookTypeRepository(context);
            await repository.AddAsync(new BookType { BookTypeName = "Biography" });
        }

        await using (var context = new LpsDbContext(options))
        {
            var saved = await context.BookTypes.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Biography", saved!.BookTypeName);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBookType()
    {
        var options = CreateOptions(nameof(UpdateAsync_UpdatesBookType));

        await using (var context = new LpsDbContext(options))
        {
            context.BookTypes.Add(new BookType { BookTypeName = "Original" });
            await context.SaveChangesAsync();
        }

        await using (var context = new LpsDbContext(options))
        {
            var repository = new BookTypeRepository(context);
            var entity = await repository.GetByIdAsync(1);
            Assert.NotNull(entity);
            entity!.BookTypeName = "Updated";
            await repository.UpdateAsync(entity);
        }

        await using (var context = new LpsDbContext(options))
        {
            var saved = await context.BookTypes.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Updated", saved!.BookTypeName);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesBookType()
    {
        var options = CreateOptions(nameof(DeleteAsync_RemovesBookType));

        await using (var context = new LpsDbContext(options))
        {
            context.BookTypes.Add(new BookType { BookTypeName = "ToDelete" });
            await context.SaveChangesAsync();
        }

        await using (var context = new LpsDbContext(options))
        {
            var repository = new BookTypeRepository(context);
            await repository.DeleteAsync(1);
        }

        await using (var context = new LpsDbContext(options))
        {
            var count = await context.BookTypes.CountAsync();
            Assert.Equal(0, count);
        }
    }
}
