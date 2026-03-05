// Core/DTOs/Auth/AccountDto.cs
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Auth;

// За login request
public class LoginRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

// За register request - махаме username
public class RegisterRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // 'user', 'supplier', 'restaurant'
}

// За login response
public class LoginResponseDto
{
    [JsonPropertyName("access")]
    public string Access { get; set; } = string.Empty;

    [JsonPropertyName("refresh")]
    public string Refresh { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public UserAccountDto User { get; set; } = new();

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("profile_id")]
    public int? ProfileId { get; set; }  // ← и това опционално
}

// За данните на account-а - махаме username
public class UserAccountDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

// За refresh token
public class RefreshTokenRequestDto
{
    [JsonPropertyName("refresh")]
    public string Refresh { get; set; } = string.Empty;
}

public class RegisterResponseDto
{
    [JsonPropertyName("access")]
    public string Access { get; set; } = string.Empty;

    [JsonPropertyName("refresh")]
    public string Refresh { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public UserAccountDto User { get; set; } = new();

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("profile_id")]
    public int? ProfileId { get; set; }
}