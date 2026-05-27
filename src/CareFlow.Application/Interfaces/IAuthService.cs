using CareFlow.Application.DTOs.Auth;

namespace CareFlow.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request);

    Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request);
}