namespace TaskManager.Application.Dtos.User;
using TaskManager.Domain.Enums;
using System;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}