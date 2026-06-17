using Moq;
using TaskManager.Application.Services.Implementations;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.IRepositories;
using Xunit;

namespace TaskManager.Tests;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _commentService = new CommentService(_commentRepositoryMock.Object);
    }

    [Fact]
    public async Task DeleteAsync_OwnerRequesting_ShouldDeleteSuccessfully()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = new Comment
        {
            Id = commentId,
            Content = "Test comment",
            UserId = userId,
            TaskItemId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
            .ReturnsAsync(comment);
        _commentRepositoryMock.Setup(repo => repo.DeleteAsync(commentId))
            .ReturnsAsync(true);

        // Act
        await _commentService.DeleteAsync(commentId, userId, UserRole.User.ToString());

        // Assert
        _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_AdminRequesting_ShouldDeleteSuccessfully()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var comment = new Comment
        {
            Id = commentId,
            Content = "Test comment",
            UserId = userId,
            TaskItemId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
            .ReturnsAsync(comment);
        _commentRepositoryMock.Setup(repo => repo.DeleteAsync(commentId))
            .ReturnsAsync(true);

        // Act
        await _commentService.DeleteAsync(commentId, adminId, UserRole.Admin.ToString());

        // Assert
        _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_OtherUserRequesting_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var comment = new Comment
        {
            Id = commentId,
            Content = "Test comment",
            UserId = userId,
            TaskItemId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
            .ReturnsAsync(comment);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _commentService.DeleteAsync(commentId, otherUserId, UserRole.User.ToString()));

        _commentRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
