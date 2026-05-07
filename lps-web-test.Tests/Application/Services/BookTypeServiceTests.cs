using lps_web_test.Domain.Entities;
using lps_web_test.Domain.Interface;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace lps_web_test.Application.Services.UnitTests
{
    public class BookTypeServiceTests
    {
        /// <summary>
        /// Verifies that constructing BookTypeService with a non-null IBookTypeRepository instance
        /// correctly wires the dependency so that subsequent GetAsync calls invoke the repository.
        /// Input conditions: a mock repository that returns a known list when GetAllAsync is called.
        /// Expected result: service.GetAsync returns the same list and repository GetAllAsync is called exactly once.
        /// </summary>
        [Fact]
        public async Task BookTypeService_Constructor_WithValidRepository_CallsRepositoryGetAllAsync()
        {
            // Arrange
            var expected = new List<BookType>
            {
                new BookType
                {
                    BookTypeName = "Fiction"
                }
            };
            var repoMock = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected).Verifiable();
            // Act
            var service = new BookTypeService(repoMock.Object);
            var result = await service.GetAsync();
            // Assert
            Assert.NotNull(result);
            Assert.Same(expected, result);
            repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        /// <summary>
        /// Verifies that constructing BookTypeService with a repository that throws does not throw at construction time,
        /// but the exception from the repository is propagated when the service method is invoked.
        /// Input conditions: a mock repository whose GetAllAsync throws InvalidOperationException.
        /// Expected result: construction succeeds; calling GetAsync throws InvalidOperationException with the same message.
        /// </summary>
        [Fact]
        public async Task BookTypeService_Constructor_WithRepositoryThatThrows_GetAsyncPropagatesException()
        {
            // Arrange
            var repoMock = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            var ex = new InvalidOperationException("repository failure");
            repoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(ex).Verifiable();
            // Act - construction should not throw
            var service = new BookTypeService(repoMock.Object);
            // Assert - calling the method propagates the repository exception
            var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAsync());
            Assert.Equal("repository failure", thrown.Message);
            repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        /// <summary>
        /// Verifies that CreateAsync forwards the provided BookType instance to the repository's AddAsync method.
        /// Input conditions: various BookType instances (including boundary Id values and long/null names).
        /// Expected result: repository.AddAsync is invoked exactly once with the same reference instance.
        /// </summary>
        [Theory]
        [MemberData(nameof(ValidBookTypes))]
        public async Task CreateAsync_ValidBookType_ForwardsToRepository(BookType bookType)
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.AddAsync(It.Is<BookType>(b => ReferenceEquals(b, bookType)))).Returns(Task.CompletedTask).Verifiable();
            var service = new BookTypeService(mockRepo.Object);
            // Act
            await service.CreateAsync(bookType);
            // Assert
            mockRepo.Verify(r => r.AddAsync(It.Is<BookType>(b => ReferenceEquals(b, bookType))), Times.Once);
        }

        /// <summary>
        /// Verifies that CreateAsync propagates exceptions thrown by the repository.
        /// Input conditions: repository.AddAsync throws InvalidOperationException.
        /// Expected result: the same InvalidOperationException is thrown by CreateAsync.
        /// </summary>
        [Fact]
        public async Task CreateAsync_RepositoryThrows_ExceptionPropagated()
        {
            // Arrange
            var bookType = new BookType
            {
                Id = 1,
                BookTypeName = "PropagationTest"
            };
            var mockRepo = new Mock<IBookTypeRepository>();
            mockRepo.Setup(r => r.AddAsync(It.Is<BookType>(b => ReferenceEquals(b, bookType)))).ThrowsAsync(new InvalidOperationException("repository failure"));
            var service = new BookTypeService(mockRepo.Object);
            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(bookType));
            Assert.Equal("repository failure", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.Is<BookType>(b => ReferenceEquals(b, bookType))), Times.Once);
        }

        /// <summary>
        /// Provides a set of BookType instances to exercise boundary and edge-case inputs.
        /// Cases included:
        /// - Default/zero id
        /// - Negative id
        /// - int.MinValue and int.MaxValue for Id
        /// - Null BookTypeName
        /// - Very long BookTypeName
        /// </summary>
        public static IEnumerable<object?[]> ValidBookTypes()
        {
            yield return new object?[]
            {
                new BookType
                {
                    Id = 0,
                    BookTypeName = "Fiction"
                }
            };
            yield return new object?[]
            {
                new BookType
                {
                    Id = -1,
                    BookTypeName = "NegativeId"
                }
            };
            yield return new object?[]
            {
                new BookType
                {
                    Id = int.MinValue,
                    BookTypeName = "MinId"
                }
            };
            yield return new object?[]
            {
                new BookType
                {
                    Id = int.MaxValue,
                    BookTypeName = "MaxId"
                }
            };
            yield return new object?[]
            {
                new BookType
                {
                    Id = 42,
                    BookTypeName = null
                }
            };
            yield return new object?[]
            {
                new BookType
                {
                    Id = 7,
                    BookTypeName = string.Empty
                }
            };
            yield return new object?[]
            {
                new BookType
                {
                    Id = 8,
                    BookTypeName = new string ('x', 5000)
                }
            };
        }

        /// <summary>
        /// Provides test cases for GetAsync: null result, empty list, and list with multiple items.
        /// </summary>
        public static IEnumerable<object?[]> GetAsync_MemberData()
        {
            yield return new object?[]
            {
                (List<BookType>? )null
            };
            yield return new object?[]
            {
                new List<BookType>()
            };
            yield return new object?[]
            {
                new List<BookType>
                {
                    new BookType(),
                    new BookType()
                }
            };
        }

        /// <summary>
        /// Tests that BookTypeService.GetAsync returns exactly what the repository returns and
        /// that the repository GetAllAsync is invoked once.
        /// Input conditions:
        /// - repositoryResult: null, empty list, or a list with items (provided via MemberData).
        /// Expected result:
        /// - If repository returns null, service returns null.
        /// - Otherwise, service returns the same list instance returned by the repository.
        /// - Repository.GetAllAsync is called exactly once.
        /// </summary>
        [Theory]
        [MemberData(nameof(GetAsync_MemberData))]
        public async Task GetAsync_RepositoryReturnsVariousResults_ReturnsExpected(List<BookType>? repositoryResult)
        {
            // Arrange
            var repositoryMock = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(repositoryResult);
            var service = new BookTypeService(repositoryMock.Object);
            // Act
            var result = await service.GetAsync();
            // Assert
            if (repositoryResult is null)
            {
                Assert.Null(result);
            }
            else
            {
                // Expect the exact instance returned by the repository (no transformation)
                Assert.Same(repositoryResult, result);
            }

            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        /// <summary>
        /// Verifies that BookTypeService.UpdateAsync forwards the exact BookType instance to the repository.
        /// Test inputs include BookType variants with boundary and special values for Id and BookTypeName:
        /// - Id = 0, BookTypeName = null
        /// - Id = 1, BookTypeName = empty string
        /// - Id = int.MaxValue, BookTypeName = long string
        /// - Id = int.MinValue, BookTypeName = string with control characters
        /// Expected: repository.UpdateAsync is invoked once with the same instance passed to the service.
        /// </summary>
        [Theory]
        [MemberData(nameof(UpdateAsync_BookTypeVariants_Data))]
        public async Task UpdateAsync_BookTypeVariants_CallsRepositoryWithSameInstance(BookType bookType)
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<BookType>())).Returns(Task.CompletedTask);
            var service = new BookTypeService(mockRepo.Object);
            // Act
            await service.UpdateAsync(bookType);
            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.Is<BookType>(b => object.ReferenceEquals(b, bookType))), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that when the repository throws an exception, BookTypeService.UpdateAsync propagates that exception.
        /// Input: a valid BookType instance.
        /// Expected: the same exception type and message is thrown by the service.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_RepositoryThrows_ExceptionIsPropagated()
        {
            // Arrange
            var bookType = new BookType
            {
                Id = 42,
                BookTypeName = "PropagationTest"
            };
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            var expected = new InvalidOperationException("update failed");
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<BookType>())).ThrowsAsync(expected);
            var service = new BookTypeService(mockRepo.Object);
            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(bookType));
            Assert.Equal(expected.Message, ex.Message);
            mockRepo.Verify(r => r.UpdateAsync(It.Is<BookType>(b => object.ReferenceEquals(b, bookType))), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Test cases for UpdateAsync variants covering boundary and special BookType values.
        /// Each object[] contains a single BookType instance to be passed to the Theory above.
        /// </summary>
        public static IEnumerable<object[]> UpdateAsync_BookTypeVariants_Data()
        {
            yield return new object[]
            {
                new BookType
                {
                    Id = 0,
                    BookTypeName = null
                }
            };
            yield return new object[]
            {
                new BookType
                {
                    Id = 1,
                    BookTypeName = string.Empty
                }
            };
            yield return new object[]
            {
                new BookType
                {
                    Id = int.MaxValue,
                    BookTypeName = new string ('a', 1000)
                }
            };
            yield return new object[]
            {
                new BookType
                {
                    Id = int.MinValue,
                    BookTypeName = "Line1\nLine2\t\u0000"
                }
            };
        }

        /// <summary>
        /// Verifies that DeleteAsync forwards the provided id to the repository and that the repository is invoked exactly once.
        /// Tests multiple boundary and typical integer values for id including int.MinValue and int.MaxValue.
        /// Expected: repository.DeleteAsync is called once with the same id and no exception is thrown.
        /// </summary>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public async Task DeleteAsync_IdValues_CallsRepositoryDeleteAsyncOnce(int id)
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            var service = new BookTypeService(mockRepo.Object);
            // Act
            await service.DeleteAsync(id);
            // Assert
            mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        /// <summary>
        /// Verifies that if the repository throws an exception during DeleteAsync the service method does not swallow it and the exception is propagated.
        /// Input condition: repository.DeleteAsync will throw InvalidOperationException for the provided id.
        /// Expected: InvalidOperationException is thrown by the service.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_RepositoryThrows_ExceptionPropagated()
        {
            // Arrange
            var testId = 42;
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new InvalidOperationException("delete failed"));
            var service = new BookTypeService(mockRepo.Object);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteAsync(testId));
            // Verify the repository was invoked with the expected id
            mockRepo.Verify(r => r.DeleteAsync(testId), Times.Once);
        }

        /// <summary>
        /// The service method under test:
        /// When search is null, empty, or whitespace, BookTypeService.GetAsync(search) should delegate
        /// to the parameterless GetAsync() path which calls repository.GetAllAsync() (no parameter)
        /// and return that list unchanged.
        /// </summary>
        /// <param name = "search">Nullable search input which may be null/empty/whitespace.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\n")]
        public async Task GetAsync_SearchIsNullOrWhiteSpace_DelegatesToParameterlessGetAsyncAndReturnsAll(string? search)
        {
            // Arrange
            var expected = new List<BookType>
            {
                new BookType
                {
                    BookTypeName = "A"
                },
                new BookType
                {
                    BookTypeName = "B"
                }
            };
            var repositoryMock = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected).Verifiable();
            // Ensure the string overload isn't called for null/empty/whitespace inputs.
            repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<string?>())).Throws(new InvalidOperationException("GetAllAsync(string?) should not be called in this case."));
            var svc = new BookTypeService(repositoryMock.Object);
            // Act
            var result = await svc.GetAsync(search);
            // Assert
            Assert.Equal(expected.Count, result.Count);
            Assert.All(expected, e => Assert.Contains(result, r => r.BookTypeName == e.BookTypeName));
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<string?>()), Times.Never);
        }

        #region EFCore Async Queryable helpers (inner classes)
        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
            {
            }

            public TestAsyncEnumerable(Expression expression) : base(expression)
            {
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return default;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }
        }

        #endregion
        /// <summary>
        /// Verifies that BookTypeExists returns false when the repository returns null for various id inputs.
        /// Inputs tested include int.MinValue, -1, 0 and int.MaxValue to exercise numeric boundaries.
        /// Expected result: method returns false (no entity found) and no exception is thrown.
        /// </summary>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task BookTypeExists_RepositoryReturnsNull_ReturnsFalse(int id)
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((BookType?)null);
            var service = new BookTypeService(mockRepo.Object);
            // Act
            var exists = await service.BookTypeExists(id);
            // Assert
            Assert.False(exists);
            mockRepo.Verify(r => r.GetByIdAsync(It.Is<int>(i => i == id)), Times.Once);
        }

        /// <summary>
        /// Verifies that BookTypeExists returns true when the repository returns an existing BookType for given ids.
        /// Tests include boundary and typical id values (int.MinValue, 1, int.MaxValue).
        /// Expected result: method returns true indicating the entity exists.
        /// </summary>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public async Task BookTypeExists_RepositoryReturnsEntity_ReturnsTrue(int id)
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int requestedId) => new BookType { Id = requestedId, BookTypeName = "Exists" });
            var service = new BookTypeService(mockRepo.Object);
            // Act
            var exists = await service.BookTypeExists(id);
            // Assert
            Assert.True(exists);
            mockRepo.Verify(r => r.GetByIdAsync(It.Is<int>(i => i == id)), Times.Once);
        }

        /// <summary>
        /// Verifies that GetByIdAsync forwards the provided id to the repository and returns the repository's value.
        /// Input conditions: various integer id values including boundaries.
        /// Expected result: returned BookType is not null and has Id equal to the provided id; repository called exactly once with that id.
        /// </summary>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public async Task GetByIdAsync_IdForwardsToRepository_ReturnsRepositoryValue(int id)
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int requestedId) => new BookType { Id = requestedId, BookTypeName = $"Name{requestedId}" });
            var service = new BookTypeService(mockRepo.Object);
            // Act
            BookType? result = await service.GetByIdAsync(id);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies behavior when repository returns null.
        /// Input conditions: repository configured to return null for the given id.
        /// Expected result: service returns null and no exception is thrown; repository called exactly once.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_RepositoryReturnsNull_ServiceReturnsNull()
        {
            // Arrange
            var id = 42;
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((BookType?)null);
            var service = new BookTypeService(mockRepo.Object);
            // Act
            BookType? result = await service.GetByIdAsync(id);
            // Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that exceptions thrown by the repository propagate through the service method.
        /// Input conditions: repository throws InvalidOperationException for the requested id.
        /// Expected result: the same InvalidOperationException is thrown by the service.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_RepositoryThrows_ExceptionPropagates()
        {
            // Arrange
            var id = 7;
            var expectedMessage = "repository failure";
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new InvalidOperationException(expectedMessage));
            var service = new BookTypeService(mockRepo.Object);
            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetByIdAsync(id));
            Assert.Equal(expectedMessage, ex.Message);
            mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that EditAsync calls IBookTypeRepository.UpdateAsync exactly once with the same BookType instance.
        /// Conditions: a valid non-null BookType instance is provided.
        /// Expected: repository.UpdateAsync invoked once and no exception is thrown.
        /// </summary>
        [Fact]
        public async Task EditAsync_ValidBookType_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>(MockBehavior.Strict);
            var bookType = new BookType
            {
                Id = 42,
                BookTypeName = "TestType"
            };
            mockRepo.Setup(r => r.UpdateAsync(It.Is<BookType>(b => b == bookType))).Returns(Task.CompletedTask).Verifiable();
            var service = new BookTypeService(mockRepo.Object);
            // Act
            await service.EditAsync(bookType);
            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.Is<BookType>(b => b == bookType)), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Ensures EditAsync propagates exceptions thrown by the repository.
        /// Conditions: repository.UpdateAsync throws InvalidOperationException for the provided BookType.
        /// Expected: EditAsync throws the same InvalidOperationException.
        /// </summary>
        [Fact]
        public async Task EditAsync_RepositoryThrows_ExceptionIsPropagated()
        {
            // Arrange
            var mockRepo = new Mock<IBookTypeRepository>();
            var bookType = new BookType
            {
                Id = 7,
                BookTypeName = "ThrowsType"
            };
            var expectedEx = new InvalidOperationException("update failed");
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<BookType>())).ThrowsAsync(expectedEx);
            var service = new BookTypeService(mockRepo.Object);
            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.EditAsync(bookType));
            Assert.Equal(expectedEx.Message, ex.Message);
            mockRepo.Verify(r => r.UpdateAsync(It.Is<BookType>(b => b == bookType)), Times.Once);
        }
    }
}