using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using lps_web_test.Domain;
using lps_web_test.Domain.Entities;
using lps_web_test.Domain.Interface;
using lps_web_test.Infrastructure;
using lps_web_test.Infrastructure.Data;
using lps_web_test.Infrastructure.Repositories;
using Microsoft;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace lps_web_test.Infrastructure.Repositories.UnitTests
{
    public class BookRepositoryTests
    {
        /// <summary>
        /// Verifies that providing a valid LpsDbContext to the BookRepository constructor
        /// creates a non-null repository instance that implements IBookRepository.
        /// Input: a newly created in-memory LpsDbContext.
        /// Expected: BookRepository instance is not null and implements IBookRepository.
        /// </summary>
        [Fact]
        public void Constructor_WithValidContext_CreatesRepositoryInstance()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act & Assert
            // Using synchronous construction here; no exception should be thrown.
            var context = new LpsDbContext(options);
            var repository = new BookRepository(context);

            Assert.NotNull(repository);
            Assert.IsAssignableFrom<IBookRepository>(repository);
        }

        /// <summary>
        /// Ensures that the repository uses the provided context instance for data access.
        /// Input: a context pre-seeded with a variable number of Book entities (0,1,3).
        /// Expected: GetAllAsync returns the same number of Book entities that were seeded.
        /// This validates that the constructor assigned the supplied context for repository use.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public async Task Constructor_AssignsContext_DataAccessibleViaRepository(int seedCount)
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            // Create and seed the same context instance that will be passed to the repository.
            await using (var context = new LpsDbContext(options))
            {
                for (int i = 0; i < seedCount; i++)
                {
                    context.Books.Add(new Book
                    {
                        BookTitle = $"Title {i}",
                        Author = $"Author {i}"
                    });
                }

                await context.SaveChangesAsync();

                // Act
                var repository = new BookRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(seedCount, result.Count);
                // If any entities exist, ensure they match seeded titles
                if (seedCount > 0)
                {
                    Assert.All(result, b => Assert.Contains("Title", b.BookTitle ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        /// <summary>
        /// Helper to create unique in-memory options per test.
        /// </summary>
        private static DbContextOptions<LpsDbContext> CreateOptions(string name)
        {
            return new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb_{name}_{Guid.NewGuid()}")
                .Options;
        }

        /// <summary>
        /// Test that GetByIdAsync returns the matching Book including its BookType when present.
        /// Arrange: seed a BookType and a Book referencing it with a specific Id.
        /// Act: call GetByIdAsync with that Id.
        /// Assert: returned Book is not null, has expected Id and the included BookType is populated.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsBookWithIncludedBookType()
        {
            // Arrange
            var options = CreateOptions(nameof(GetByIdAsync_ExistingId_ReturnsBookWithIncludedBookType));

            await using (var context = new LpsDbContext(options))
            {
                var bookType = new BookType { Id = 10, BookTypeName = "Fiction" };
                var book = new Book
                {
                    Id = 5,
                    BookTitle = "Title",
                    Author = "Author",
                    BookTypeId = 10,
                    BookType = bookType
                };

                context.BookTypes.Add(bookType);
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            Book? result;
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                result = await repository.GetByIdAsync(5);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result!.Id);
            Assert.NotNull(result.BookType);
            Assert.Equal("Fiction", result.BookType!.BookTypeName);
        }

        /// <summary>
        /// Tests that GetByIdAsync returns null for ids that are not present in the database.
        /// Arrange: seed a single book with Id = 1.
        /// Act: call GetByIdAsync with a variety of non-existing id values (including boundary ints).
        /// Assert: result is null for each non-existing id.
        /// </summary>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public async Task GetByIdAsync_NonExistingIds_ReturnsNull(int id)
        {
            // Arrange
            var options = CreateOptions(nameof(GetByIdAsync_NonExistingIds_ReturnsNull));

            await using (var context = new LpsDbContext(options))
            {
                var existing = new Book { Id = 1, BookTitle = "Existing", Author = "A" };
                context.Books.Add(existing);
                await context.SaveChangesAsync();
            }

            // Act
            Book? result;
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                result = await repository.GetByIdAsync(id);
            }

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Test that GetByIdAsync returns a Book whose BookType is null when the BookType is not present (BookTypeId is null).
        /// Arrange: seed a Book with BookTypeId = null.
        /// Act: call GetByIdAsync for that Book's Id.
        /// Assert: Book is returned and its BookType navigation property is null.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_BookWithoutBookType_ReturnsBookWithNullBookType()
        {
            // Arrange
            var options = CreateOptions(nameof(GetByIdAsync_BookWithoutBookType_ReturnsBookWithNullBookType));

            await using (var context = new LpsDbContext(options))
            {
                var book = new Book
                {
                    Id = 7,
                    BookTitle = "Orphan",
                    Author = "Nobody",
                    BookTypeId = null,
                    BookType = null
                };
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            // Act
            Book? result;
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                result = await repository.GetByIdAsync(7);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result!.Id);
            Assert.Null(result.BookType);
        }

        /// <summary>
        /// The test verifies that AddAsync calls DbSet.Add and SaveChangesAsync when provided with books
        /// having various BookTitle values (including null, empty, whitespace and very long strings).
        /// Expected: The same instance passed to AddAsync is forwarded to the DbSet.Add call and SaveChangesAsync is invoked once.
        /// </summary>
        [Theory]
        [MemberData(nameof(BookTitleTestData))]
        public async Task AddAsync_BookWithVariousTitles_AddsAndSaves(string? title)
        {
            // Arrange
            var options = new DbContextOptions<LpsDbContext>();
            var mockContext = new Mock<LpsDbContext>(options);
            var mockSet = new Mock<DbSet<Book>>();

            Book? captured = null;
            mockSet
                .Setup(s => s.Add(It.IsAny<Book>()))
                .Callback<Book>(b => captured = b)
                .Returns((Book b) => (EntityEntry<Book>?)null);

            mockContext.Setup(c => c.Books).Returns(mockSet.Object);
            mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var repository = new BookRepository(mockContext.Object);

            var book = new Book
            {
                BookTitle = title,
                Author = "AuthorName"
            };

            // Act
            await repository.AddAsync(book);

            // Assert
            Assert.Same(book, captured);
            mockSet.Verify(s => s.Add(It.Is<Book>(x => ReferenceEquals(x, book))), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        public static IEnumerable<object?[]> BookTitleTestData()
        {
            yield return new object?[] { null };
            yield return new object?[] { string.Empty };
            yield return new object?[] { "   " };
            yield return new object?[] { "Normal Title" };
            yield return new object?[] { new string('x', 5000) };
            yield return new object?[] { "TitleWith\nControlChar\u0001" };
        }

        /// <summary>
        /// The test verifies that when the DbContext.SaveChangesAsync throws an exception, AddAsync propagates that exception.
        /// Input: a valid Book instance and a mocked DbContext configured to throw on SaveChangesAsync.
        /// Expected: The same exception type is thrown and DbSet.Add was still invoked prior to the exception.
        /// </summary>
        [Fact]
        public async Task AddAsync_SaveChangesThrows_PropagatesException()
        {
            // Arrange
            var options = new DbContextOptions<LpsDbContext>();
            var mockContext = new Mock<LpsDbContext>(options);
            var mockSet = new Mock<DbSet<Book>>();

            Book? captured = null;
            mockSet
                .Setup(s => s.Add(It.IsAny<Book>()))
                .Callback<Book>(b => captured = b)
                .Returns((Book b) => (EntityEntry<Book>?)null);

            mockContext.Setup(c => c.Books).Returns(mockSet.Object);
            mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("DB save failure"));

            var repository = new BookRepository(mockContext.Object);

            var book = new Book
            {
                BookTitle = "WillFailSave",
                Author = "AuthorName"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.AddAsync(book));
            Assert.Equal("DB save failure", ex.Message);

            // Ensure Add was called even though SaveChangesAsync failed
            mockSet.Verify(s => s.Add(It.Is<Book>(x => ReferenceEquals(x, book))), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Same(book, captured);
        }

        /// <summary>
        /// Provides null, empty and whitespace-only search inputs to verify the query is unfiltered.
        /// </summary>
        public static IEnumerable<object?[]> NullOrWhiteSpaceSearchValues()
        {
            yield return new object?[] { null };
            yield return new object?[] { string.Empty };
            yield return new object?[] { "   " };
        }

        /// <summary>
        /// Provides non-empty search inputs including special and very long strings to verify WHERE+pattern is applied.
        /// </summary>
        public static IEnumerable<object?[]> NonEmptySearchValues()
        {
            yield return new object?[] { "alpha" };
            yield return new object?[] { "A B" };
            yield return new object?[] { "%_special_% " };
            yield return new object?[] { new string('x', 512) }; // very long
            yield return new object?[] { "\0\n\r\t" }; // control characters
        }

        /// <summary>
        /// The test ensures that when search is null or whitespace the returned IQueryable does not include a Where clause.
        /// Inputs: null, empty string, whitespace-only string.
        /// Expected: The expression tree for the returned IQueryable contains no 'Where' method call.
        /// </summary>
        [Theory]
        [MemberData(nameof(NullOrWhiteSpaceSearchValues))]
        public void GetAllAsync_SearchNullOrWhiteSpace_ReturnsUnfilteredQueryable(string? search)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Seed data
            using (var context = new LpsDbContext(options))
            {
                context.Books.AddRange(
                    new Book { BookTitle = "First", Author = "Author1" },
                    new Book { BookTitle = "Second", Author = "Author2" });
                context.SaveChanges();
            }

            using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);

                // Act
                var query = repository.GetAllAsync(search);

                // Assert
                Assert.NotNull(query);
                Assert.False(ExpressionContainsWhere(query.Expression),
                    $"Expected no Where in expression for search='{search ?? "null"}', but found one. Expression: {query.Expression}");
            }
        }

        /// <summary>
        /// The test ensures that when search is a non-empty string a WHERE clause using EF.Functions.Like is added and the LIKE pattern includes the search term.
        /// Inputs: several non-empty strings (normal, special-chars, very long, control-chars).
        /// Expected: The expression tree contains a 'Where' method call and a constant pattern "%{search}%".
        /// Note: The test inspects the expression tree only and does not execute the query to avoid provider-specific translation issues.
        /// </summary>
        [Theory]
        [MemberData(nameof(NonEmptySearchValues))]
        public void GetAllAsync_NonEmptySearch_AppliesWhereWithLikePattern(string search)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LpsDbContext(options))
            {
                context.Books.AddRange(
                    new Book { BookTitle = "FirstMatch", Author = "Author1" },
                    new Book { BookTitle = "Second", Author = "Author2" });
                context.SaveChanges();
            }

            using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);

                // Act
                var query = repository.GetAllAsync(search);

                // Assert
                Assert.NotNull(query);
                Assert.True(ExpressionContainsWhere(query.Expression),
                    $"Expected Where in expression for search='{search}', but none found. Expression: {query.Expression}");

                var expectedPattern = $"%{search}%";
                var constantStrings = CollectStringConstants(query.Expression).ToList();
                Assert.Contains(constantStrings, s => s == expectedPattern);
            }
        }

        // Helper: checks if any MethodCallExpression in the tree has Method.Name == "Where"
        private static bool ExpressionContainsWhere(Expression expr)
        {
            if (expr is MethodCallExpression mce)
            {
                if (string.Equals(mce.Method.Name, "Where", StringComparison.Ordinal))
                    return true;

                foreach (var arg in mce.Arguments)
                {
                    if (ExpressionContainsWhere(arg))
                        return true;
                }

                if (mce.Object != null && ExpressionContainsWhere(mce.Object))
                    return true;
            }
            else if (expr is UnaryExpression ue)
            {
                return ExpressionContainsWhere(ue.Operand);
            }
            else if (expr is LambdaExpression le)
            {
                return ExpressionContainsWhere(le.Body);
            }
            else if (expr is BinaryExpression be)
            {
                return ExpressionContainsWhere(be.Left) || ExpressionContainsWhere(be.Right);
            }
            else if (expr is MemberExpression me)
            {
                return me.Expression != null && ExpressionContainsWhere(me.Expression);
            }

            return false;
        }

        // Helper: collects string constants in the expression tree (used to find the LIKE pattern constant)
        private static IEnumerable<string> CollectStringConstants(Expression expr)
        {
            var results = new List<string>();
            Collect(expr, results);
            return results;

            static void Collect(Expression? e, List<string> acc)
            {
                if (e == null) return;

                if (e is ConstantExpression ce && ce.Value is string s)
                {
                    acc.Add(s);
                    return;
                }

                if (e is MethodCallExpression mce)
                {
                    foreach (var a in mce.Arguments) Collect(a, acc);
                    if (mce.Object != null) Collect(mce.Object, acc);
                    return;
                }

                if (e is UnaryExpression ue)
                {
                    Collect(ue.Operand, acc);
                    return;
                }

                if (e is LambdaExpression le)
                {
                    Collect(le.Body, acc);
                    return;
                }

                if (e is BinaryExpression be)
                {
                    Collect(be.Left, acc);
                    Collect(be.Right, acc);
                    return;
                }

                if (e is MemberExpression me)
                {
                    if (me.Type == typeof(string))
                    {
                        var evaluated = EvaluateExpression(me);
                        if (evaluated is string evaluatedString)
                        {
                            acc.Add(evaluatedString);
                        }
                    }

                    Collect(me.Expression, acc);
                    return;
                }

                if (e is NewArrayExpression nae)
                {
                    foreach (var ex in nae.Expressions) Collect(ex, acc);
                }

                if (e is InvocationExpression ie)
                {
                    Collect(ie.Expression, acc);
                    foreach (var arg in ie.Arguments) Collect(arg, acc);
                }
            }
        }

        private static object? EvaluateExpression(Expression expr)
        {
            if (expr is ConstantExpression ce)
                return ce.Value;

            if (expr is MemberExpression me)
            {
                var instance = me.Expression is null ? null : EvaluateExpression(me.Expression);
                if (instance == null)
                    return null;

                return me.Member switch
                {
                    FieldInfo field => field.GetValue(instance),
                    PropertyInfo property => property.GetValue(instance),
                    _ => null
                };
            }

            if (expr is UnaryExpression ue && ue.NodeType == ExpressionType.Convert)
                return EvaluateExpression(ue.Operand);

            return null;
        }

        /// <summary>
        /// Verifies that GetAllAsync returns an empty list when the database contains no books.
        /// Input: an empty database.
        /// Expected: an empty list returned and no exceptions thrown.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_NoBooks_ReturnsEmptyList()
        {
            // Arrange
            var options = CreateOptions(nameof(GetAllAsync_NoBooks_ReturnsEmptyList));
            await using (var context = new LpsDbContext(options))
            {
                // no seed data
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        /// <summary>
        /// Verifies that GetAllAsync returns the expected number of books and that each returned Book has its BookType navigation property loaded when associated.
        /// Input: database seeded with N books all associated to a BookType.
        /// Expected: result count equals N and every Book.BookType is not null.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task GetAllAsync_WithBooks_IncludesBookType_ReturnsExpectedCountAndNavigationLoaded(int bookCount)
        {
            // Arrange
            var dbName = $"{nameof(GetAllAsync_WithBooks_IncludesBookType_ReturnsExpectedCountAndNavigationLoaded)}_{bookCount}_{Guid.NewGuid()}";
            var options = CreateOptions(dbName);

            // Seed data in one context instance
            await using (var seedContext = new LpsDbContext(options))
            {
                var type = new BookType { BookTypeName = "TestType" };
                seedContext.BookTypes.Add(type);

                for (var i = 1; i <= bookCount; i++)
                {
                    seedContext.Books.Add(new Book
                    {
                        BookTitle = $"Title {i}",
                        Author = $"Author {i}",
                        BookType = type
                    });
                }

                await seedContext.SaveChangesAsync();
            }

            // Act & Assert in a fresh context to ensure query executes against DB (and include is honored)
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                var result = await repository.GetAllAsync();

                Assert.NotNull(result);
                Assert.Equal(bookCount, result.Count);
                Assert.All(result, b => Assert.NotNull(b.BookType));
                // Validate at least one title was persisted when count > 0
                if (bookCount > 0)
                {
                    Assert.Contains(result, b => b.BookTitle != null && b.BookTitle.StartsWith("Title"));
                }
            }
        }

        /// <summary>
        /// Verifies that GetAllAsync returns books even when some books have no associated BookType.
        /// Input: database seeded with one book that has null BookType and null BookTypeId.
        /// Expected: the returned Book exists and its BookType property is null.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_BookWithoutType_ReturnsBookWithNullBookType()
        {
            // Arrange
            var options = CreateOptions(nameof(GetAllAsync_BookWithoutType_ReturnsBookWithNullBookType));
            await using (var seedContext = new LpsDbContext(options))
            {
                seedContext.Books.Add(new Book
                {
                    BookTitle = "Orphan Book",
                    Author = "NoType Author",
                    BookType = null,
                    BookTypeId = null
                });

                await seedContext.SaveChangesAsync();
            }

            // Act
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Single(result);
                var book = result.First();
                Assert.Equal("Orphan Book", book.BookTitle);
                Assert.Null(book.BookType);
            }
        }

        /// <summary>
        /// Tests that when the search parameter is null, empty, or whitespace-only,
        /// GetCountAsync does not apply filtering and returns the total number of books.
        /// Input conditions: search is null, empty string, or whitespace-only string.
        /// Expected result: returns the total seeded count of books.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetCountAsync_NullOrWhitespace_ReturnsTotalCount(string? search)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using (var seedContext = new LpsDbContext(options))
            {
                seedContext.Books.AddRange(
                    new Book { BookTitle = "Science Fiction", Author = "Author A" },
                    new Book { BookTitle = "Fictional Tale", Author = "Author B" },
                    new Book { BookTitle = "Rowling's Story", Author = "J.K. Rowling" }
                );
                await seedContext.SaveChangesAsync();
            }

            // Act
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                var count = await repository.GetCountAsync(search);

                // Assert
                Assert.Equal(3, count);
            }
        }

        /// <summary>
        /// Parameterized tests exercising various search inputs for GetCountAsync.
        /// Inputs include substring matches, wildcard-like input ("%"), nonexistent long strings,
        /// and a simple non-matching value. Expected result: count reflecting EF.Functions.Like("%{search}%")
        /// semantics against BookTitle and Author.
        /// </summary>
        [Theory]
        [InlineData("Rowling", 1)]
        [InlineData("Fiction", 2)]
        [InlineData("%", 3)]                // '%' as search becomes '%%' pattern -> matches all in SQL-like semantics
        [InlineData("nonexistent", 0)]
        [InlineData("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", 0)] // very long string unlikely to match
        public async Task GetCountAsync_SearchString_ReturnsExpectedCount(string search, int expectedCount)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LpsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using (var seedContext = new LpsDbContext(options))
            {
                seedContext.Books.AddRange(
                    new Book { BookTitle = "Science Fiction", Author = "Author A" },
                    new Book { BookTitle = "Fictional Tale", Author = "Author B" },
                    new Book { BookTitle = "Rowling's Story", Author = "J.K. Rowling" }
                );
                await seedContext.SaveChangesAsync();
            }

            // Act
            await using (var context = new LpsDbContext(options))
            {
                var repository = new BookRepository(context);
                var count = await repository.GetCountAsync(search);

                // Assert
                Assert.Equal(expectedCount, count);
            }
        }
    }
}