namespace TaskManager.Domain.IServices;

public interface ICalendarService
{
    Task CreateEventAsync(Guid userId, string title, string description, DateTime? dueDate);
    Task<string> GetGoogleAuthorizationUrlAsync(Guid userId, string redirectUri);
    Task ConnectGoogleAccountAsync(Guid userId, string code, string redirectUri);
}
