using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using TaskManager.Domain.IRepositories;
using TaskManager.Domain.IServices;

namespace TaskManager.Infrastructure.GoogleCalendar;

public class GoogleCalendarService : IGoogleCalendar
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public GoogleCalendarService(
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task CreateEventAsync(Guid userId, string title, string description, DateTime? dueDate)
    {
        if (dueDate == null) return;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.GoogleRefreshToken))
        {
            // O usuário não possui conta Google vinculada ou não existe
            return;
        }

        var clientId = _configuration["Google:ClientId"];
        var clientSecret = _configuration["Google:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            // Configurações do Google não fornecidas no appsettings
            return;
        }

        var clientSecrets = new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };

        var token = new TokenResponse
        {
            AccessToken = user.GoogleAccessToken,
            RefreshToken = user.GoogleRefreshToken
        };

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets
        });

        var credential = new UserCredential(flow, user.Id.ToString(), token);

        try
        {
            // Força a renovação se o AccessToken estiver expirado
            if (credential.Token.IsExpired(Google.Apis.Util.SystemClock.Default) || string.IsNullOrEmpty(credential.Token.AccessToken))
            {
                var refreshed = await credential.RefreshTokenAsync(CancellationToken.None);
                if (refreshed)
                {
                    user.GoogleAccessToken = credential.Token.AccessToken;
                    await _userRepository.UpdateAsync(user);
                }
            }

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "TaskManagerAPI"
            });

            var calendarEvent = new Event
            {
                Summary = title,
                Description = description,
                Start = new EventDateTime { DateTimeDateTimeOffset = dueDate.Value },
                End = new EventDateTime { DateTimeDateTimeOffset = dueDate.Value.AddHours(1) }
            };

            await service.Events.Insert(calendarEvent, "primary").ExecuteAsync();
        }
        catch
        {
            // Se der erro de autenticação ou API, falha silenciosamente para não quebrar a criação da task
        }
    }

    public async Task<string> GetGoogleAuthorizationUrlAsync(Guid userId, string redirectUri)
    {
        var clientId = _configuration["Google:ClientId"];
        var clientSecret = _configuration["Google:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            throw new InvalidOperationException("Google credentials are not configured.");

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = new[] { CalendarService.Scope.Calendar }
        });

        var requestUrl = new GoogleAuthorizationCodeRequestUrl(new Uri(flow.AuthorizationServerUrl))
        {
            ClientId = clientId,
            RedirectUri = redirectUri,
            Scope = string.Join(" ", flow.Scopes),
            State = userId.ToString(),
            AccessType = "offline",
            ApprovalPrompt = "force"
        };

        return await Task.FromResult(requestUrl.Build().ToString());
    }

    public async Task ConnectGoogleAccountAsync(Guid userId, string code, string redirectUri)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found.");

        var clientId = _configuration["Google:ClientId"];
        var clientSecret = _configuration["Google:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            throw new InvalidOperationException("Google credentials are not configured.");

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = new[] { CalendarService.Scope.Calendar }
        });

        var tokenResponse = await flow.ExchangeCodeForTokenAsync(userId.ToString(), code, redirectUri, CancellationToken.None);

        user.GoogleAccessToken = tokenResponse.AccessToken;
        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            user.GoogleRefreshToken = tokenResponse.RefreshToken;
        }

        await _userRepository.UpdateAsync(user);
    }
}
